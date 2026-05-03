using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Commands;
using Domain.Entities;
using LanguageExt;
using Moq;
using Xunit;

namespace Application.Tests.Users.Commands;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock = new();
    private readonly Mock<IUserQueries> _userQueriesMock = new();
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _handler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object,
            _userQueriesMock.Object);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsUserNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UserNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUpdatedUser()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateUser();

        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsRight);
    }

    [Fact]
    public async Task Handle_WhenUserExists_CallsUpdateOnRepository()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateUser();

        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _userRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsUnhandledUserException()
    {
        // Arrange
        var command = CreateValidCommand();
        var user = CreateUser();

        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _userRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UnhandledUserException>(ex));
    }

    private static UpdateUserCommand CreateValidCommand() => new()
    {
        Id = 1,
        UserName = "updateduser",
        Email = "updated@example.com"
    };

    private static User CreateUser() =>
        User.New("testuser", "test@email.com", "hash", Domain.UserRole.User);
}