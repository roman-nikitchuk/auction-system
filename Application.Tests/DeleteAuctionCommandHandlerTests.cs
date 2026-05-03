using Application.Auctions.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using Moq;
using Xunit;

namespace Application.Tests.Auctions.Commands;

public class DeleteAuctionCommandHandlerTests
{
    private readonly Mock<IRepository<Auction>> _auctionRepositoryMock = new();
    private readonly Mock<IAuctionQueries> _auctionQueriesMock = new();
    private readonly DeleteAuctionCommandHandler _handler;

    public DeleteAuctionCommandHandlerTests()
    {
        _handler = new DeleteAuctionCommandHandler(
            _auctionRepositoryMock.Object,
            _auctionQueriesMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAuctionDoesNotExist_ReturnsAuctionNotFoundException()
    {
        var command = new DeleteAuctionCommand { Id = 999 };

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.None);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<AuctionNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenAuctionExists_ReturnsDeletedAuction()
    {
        var auction = CreateAuction();
        var command = new DeleteAuctionCommand { Id = 1 };

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));

        _auctionRepositoryMock
            .Setup(x => x.DeleteAsync(auction, It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsRight);
    }

    [Fact]
    public async Task Handle_WhenAuctionExists_CallsDeleteOnRepository()
    {
        var auction = CreateAuction();
        var command = new DeleteAuctionCommand { Id = 1 };

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));

        _auctionRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(auction);

        await _handler.Handle(command, CancellationToken.None);

        _auctionRepositoryMock.Verify(
            x => x.DeleteAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrows_ReturnsUnhandledAuctionException()
    {
        var auction = CreateAuction();
        var command = new DeleteAuctionCommand { Id = 1 };

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(auction));

        _auctionRepositoryMock
            .Setup(x => x.DeleteAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<UnhandledAuctionException>(ex));
    }

    private static Auction CreateAuction() =>
        Auction.New(1, "Test", "Desc", 1, 100m,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(7));
}