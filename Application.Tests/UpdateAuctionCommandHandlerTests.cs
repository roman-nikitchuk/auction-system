using Application.Auctions.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using Moq;
using Xunit;

namespace Application.Tests.Auctions.Commands;

public class UpdateAuctionCommandHandlerTests
{
    private readonly Mock<IRepository<Auction>> _auctionRepositoryMock = new();
    private readonly Mock<IAuctionQueries> _auctionQueriesMock = new();
    private readonly Mock<ICategoryQueries> _categoryQueriesMock = new();
    private readonly UpdateAuctionCommandHandler _handler;

    public UpdateAuctionCommandHandlerTests()
    {
        _handler = new UpdateAuctionCommandHandler(
            _auctionRepositoryMock.Object,
            _auctionQueriesMock.Object,
            _categoryQueriesMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAuctionDoesNotExist_ReturnsAuctionNotFoundException()
    {
        var command = CreateValidCommand();

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.None);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<AuctionNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenAuctionIsNotActive_ReturnsAuctionNotActiveException()
    {
        var command = CreateValidCommand();

        var expiredAuction = Auction.New(1, "Old", "Desc", 1, 100m,
            DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-1));

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(expiredAuction));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<AuctionNotActiveException>(ex));
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsCategoryNotFoundException()
    {
        var command = CreateValidCommand();
        var activeAuction = CreateActiveAuction();

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(activeAuction));

        _categoryQueriesMock
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Category>.None);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsLeft);
        result.IfLeft(ex => Assert.IsType<CategoryNotFoundException>(ex));
    }

    [Fact]
    public async Task Handle_WhenValid_ReturnsUpdatedAuction()
    {
        var command = CreateValidCommand();
        var activeAuction = CreateActiveAuction();
        var category = Category.New("Test");

        _auctionQueriesMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Auction>.Some(activeAuction));

        _categoryQueriesMock
            .Setup(x => x.GetByIdAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Option<Category>.Some(category));

        _auctionRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeAuction);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsRight);

        _auctionRepositoryMock.Verify(
            x => x.UpdateAsync(It.IsAny<Auction>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static UpdateAuctionCommand CreateValidCommand() => new()
    {
        Id = 1,
        Title = "Updated Title",
        Description = "Updated Description",
        CategoryId = 1,
        StartDate = DateTime.UtcNow.AddDays(1),
        EndDate = DateTime.UtcNow.AddDays(7)
    };

    private static Auction CreateActiveAuction() =>
        Auction.New(1, "Test", "Desc", 1, 100m,
            DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(7));
}