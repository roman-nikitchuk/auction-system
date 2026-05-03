using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Users.Commands;
using Domain;
using Domain.Entities;
using LanguageExt;
using Moq;
using Xunit;

namespace Application.Tests.Users.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock = new();
    private readonly Mock<IUserQueries> _userQueriesMock = new();
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _userQueriesMock.Object);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsUserAlreadyExistsException()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingUser = User.New("existing", command.Email, "hash", UserRole.User);

        _userQueriesMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(existingUser));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UserAlreadyExistsException>(ex));
    }

    [Fact]
    public async Task Handle_WhenEmailIsUnique_ReturnsCreatedUser()
    {
        // Arrange
        var command = CreateValidCommand();
        var expectedUser = User.New(command.UserName, command.Email, "hashed_password", UserRole.User);

        _userQueriesMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);
        _userQueriesMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { expectedUser }); // not first user
        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsRight);
        result.IfRight(user => Assert.Equal(command.Email, user.Email));
    }

    [Fact]
    public async Task Handle_WhenFirstUser_AssignsAdminRole()
    {
        // Arrange
        var command = CreateValidCommand();
        User? createdUser = null;

        _userQueriesMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);
        _userQueriesMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>()); // empty → first user
        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => createdUser = u)
            .ReturnsAsync((User u, CancellationToken _) => u);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(createdUser);
        Assert.Equal(UserRole.Admin, createdUser!.Role);
    }

    [Fact]
    public async Task Handle_WhenNotFirstUser_AssignsUserRole()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingUsers = new List<User> { User.New("other", "other@email.com", "hash", UserRole.Admin) };
        User? createdUser = null;

        _userQueriesMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);
        _userQueriesMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUsers);
        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => createdUser = u)
            .ReturnsAsync((User u, CancellationToken _) => u);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(createdUser);
        Assert.Equal(UserRole.User, createdUser!.Role);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsUnhandledUserException()
    {
        // Arrange
        var command = CreateValidCommand();

        _userQueriesMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);
        _userQueriesMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());
        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UnhandledUserException>(ex));
    }

    [Fact]
    public async Task Handle_PasswordIsSavedAsHash_NotPlainText()
    {
        // Arrange
        var command = CreateValidCommand();
        User? createdUser = null;

        _userQueriesMock
            .Setup(x => x.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);
        _userQueriesMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());
        _userRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => createdUser = u)
            .ReturnsAsync((User u, CancellationToken _) => u);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert — password must NOT be stored as plain text
        Assert.NotNull(createdUser);
        Assert.NotEqual(command.Password, createdUser!.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify(command.Password, createdUser.PasswordHash));
    }

    // --- Helpers ---
    private static CreateUserCommand CreateValidCommand() => new()
    {
        UserName = "testuser",
        Email = "test@example.com",
        Password = "SecurePass123"
    };
}