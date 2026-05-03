using Application.Bids.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using Moq;
using Xunit;

namespace Application.Tests.Bids.Commands;

public class CreateBidCommandHandlerTests
{
    private readonly Mock<IRepository<Bid>> _bidRepositoryMock = new();
    private readonly Mock<IAuctionQueries> _auctionQueriesMock = new();
    private readonly Mock<IUserQueries> _userQueriesMock = new();
    private readonly Mock<IRepository<Auction>> _auctionRepositoryMock = new();
    private readonly CreateBidCommandHandler _handler;

    public CreateBidCommandHandlerTests()
    {
        _handler = new CreateBidCommandHandler(
            _bidRepositoryMock.Object,
            _auctionQueriesMock.Object,
            _userQueriesMock.Object,
            _auctionRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAuctionDoesNotExist_ReturnsAuctionNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.AuctionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.None);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<AuctionNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsUserNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        var auction = CreateActiveAuction(ownerId: 99);

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.AuctionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.None);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UserNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenAuctionIsNotActive_ReturnsAuctionNotActiveException()
    {
        // Arrange
        var command = CreateValidCommand();
        var auction = CreateExpiredAuction(ownerId: 99);
        var user = CreateUser(command.UserId);

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.AuctionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<AuctionNotActiveException>(ex));
    }

    [Fact]
    public async Task Handle_WhenUserIsOwner_ReturnsBidOnOwnAuctionException()
    {
        // Arrange
        const int ownerId = 1;
        var command = CreateValidCommand(userId: ownerId);
        var auction = CreateActiveAuction(ownerId: ownerId);
        var user = CreateUser(ownerId);

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.AuctionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<BidOnOwnAuctionException>(ex));
    }

    [Fact]
    public async Task Handle_WhenAmountTooLow_ReturnsBidTooLowException()
    {
        // Arrange
        var command = CreateValidCommand(amount: 50m); // current bid is 100m
        var auction = CreateActiveAuction(ownerId: 99, currentBid: 100m);
        var user = CreateUser(command.UserId);

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.AuctionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<BidTooLowException>(ex));
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsCreatedBid()
    {
        // Arrange
        var command = CreateValidCommand(amount: 200m);
        var auction = CreateActiveAuction(ownerId: 99, currentBid: 100m);
        var user = CreateUser(command.UserId);
        var expectedBid = Bid.New(command.AuctionId, command.UserId, command.Amount);

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.AuctionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _bidRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Bid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBid);
        _auctionRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsRight);
        result.IfRight(bid => Assert.Equal(command.Amount, bid.Amount));
    }

    [Fact]
    public async Task Handle_WhenValid_UpdatesAuctionCurrentBid()
    {
        // Arrange
        var command = CreateValidCommand(amount: 200m);
        var auction = CreateActiveAuction(ownerId: 99, currentBid: 100m);
        var user = CreateUser(command.UserId);

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.AuctionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));
        _userQueriesMock
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<User>.Some(user));
        _bidRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Bid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Bid.New(command.AuctionId, command.UserId, command.Amount));
        _auctionRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert — auction.UpdateCurrentBid was called → UpdateAsync was called
        _auctionRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // --- Helpers ---
    private static CreateBidCommand CreateValidCommand(int userId = 2, decimal amount = 200m) => new()
    {
        AuctionId = 1,
        UserId = userId,
        Amount = amount
    };

    private static Auction CreateActiveAuction(int ownerId, decimal currentBid = 100m)
    {
        var a = Auction.New(ownerId, "Test", "Desc", 1, currentBid,
            DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(7));
        return a;
    }

    private static Auction CreateExpiredAuction(int ownerId)
    {
        var a = Auction.New(ownerId, "Test", "Desc", 1, 100m,
            DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-1)); // ended yesterday
        return a;
    }

    private static User CreateUser(int id) =>
        User.New("user" + id, $"user{id}@email.com", "hash", Domain.UserRole.User);
}