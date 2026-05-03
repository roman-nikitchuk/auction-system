using Application.Auctions.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using Moq;
using Xunit;

namespace Application.Tests.Auctions.Commands;

public class CreateAuctionCommandHandlerTests
{
    private readonly Mock<IRepository<Auction>> _auctionRepositoryMock = new();
    private readonly Mock<IUserQueries> _userQueriesMock = new();
    private readonly Mock<ICategoryQueries> _categoryQueriesMock = new();
    private readonly CreateAuctionCommandHandler _handler;

    public CreateAuctionCommandHandlerTests()
    {
        _handler = new CreateAuctionCommandHandler(
            _auctionRepositoryMock.Object,
            _userQueriesMock.Object,
            _categoryQueriesMock.Object);
    }

    [Fact]
    public async Task Handle_WhenOwnerDoesNotExist_ReturnsUserNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UserNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsCategoryNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateUser();

        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _categoryQueriesMock
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Category>.None);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<CategoryNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsCreatedAuction()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateUser();
        var category = CreateCategory();
        var expectedAuction = CreateAuction(command);

        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _categoryQueriesMock
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Category>.Some(category));
        _auctionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAuction);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsRight);
        result.IfRight(auction => Assert.Equal(expectedAuction.Title, auction.Title));
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsUnhandledAuctionException()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateUser();
        var category = CreateCategory();

        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.OwnerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _categoryQueriesMock
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Category>.Some(category));
        _auctionRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UnhandledAuctionException>(ex));
    }

    // --- Helpers ---
    private static CreateAuctionCommand CreateValidCommand() => new()
    {
        Title = "Test Auction",
        Description = "Test Description",
        CategoryId = 1,
        StartingPrice = 100m,
        StartDate = DateTime.UtcNow.AddDays(1),
        EndDate = DateTime.UtcNow.AddDays(7),
        OwnerId = 1
    };

    private static User CreateUser() => User.New("testuser", "test@email.com", "hash", Domain.UserRole.User);
   private static Category CreateCategory() => Category.New("Electronics");
    private static Auction CreateAuction(CreateAuctionCommand cmd) =>
        Auction.New(cmd.OwnerId, cmd.Title, cmd.Description, cmd.CategoryId, cmd.StartingPrice, cmd.StartDate, cmd.EndDate);
}