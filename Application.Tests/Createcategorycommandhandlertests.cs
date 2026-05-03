using Application.Categories.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using LanguageExt;
using Moq;
using Xunit;
using CategoryEntity = Domain.Entities.Category;

namespace Application.Tests.Categories.Commands;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<IRepository<CategoryEntity>> _categoryRepositoryMock = new();
    private readonly Mock<ICategoryQueries> _categoryQueriesMock = new();
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _handler = new CreateCategoryCommandHandler(
            _categoryRepositoryMock.Object,
            _categoryQueriesMock.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryNameAlreadyExists_ReturnsCategoryAlreadyExistsException()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Electronics" };
        var existing = CategoryEntity.New("Electronics");

        _categoryQueriesMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<CategoryEntity>.Some(existing));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<CategoryAlreadyExistsException>(ex));
    }

    [Fact]
    public async Task Handle_WhenCategoryNameIsUnique_ReturnsCreatedCategory()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Books" };
        var expected = CategoryEntity.New("Books");

        _categoryQueriesMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<CategoryEntity>.None);
        _categoryRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsRight);
        result.IfRight(cat => Assert.Equal("Books", cat.Name));
    }

    [Fact]
    public async Task Handle_WhenCategoryNameIsUnique_CallsCreateOnRepository()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Books" };

        _categoryQueriesMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<CategoryEntity>.None);
        _categoryRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CategoryEntity.New("Books"));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _categoryRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsUnhandledCategoryException()
    {
        // Arrange
        var command = new CreateCategoryCommand { Name = "Books" };

        _categoryQueriesMock
            .Setup(x => x.GetByNameAsync(command.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<CategoryEntity>.None);
        _categoryRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<CategoryEntity>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UnhandledCategoryException>(ex));
    }
}