using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using MediatR;

namespace Application.Bids.Commands;

public class CreateBidCommand : IRequest<Either<BaseException, Bid>>
{
    public required int AuctionId { get; init; }
    public required int UserId { get; init; }
    public required decimal Amount { get; init; }
}

public class CreateBidCommandHandler(
    IRepository<Bid> bidRepository,
    IAuctionQueries auctionQueries,
    IUserQueries userQueries,
    IRepository<Auction> auctionRepository) : IRequestHandler<CreateBidCommand, Either<BaseException, Bid>>
{
    public async Task<Either<BaseException, Bid>> Handle(
        CreateBidCommand request, CancellationToken cancellationToken)
    {
        var auction = await auctionQueries.GetByIdAsync(request.AuctionId, cancellationToken);
        if (auction.IsNone)
            return new AuctionNotFoundException(request.AuctionId);

        var user = await userQueries.GetByIdAsync(request.UserId, cancellationToken);
        if (user.IsNone)
            return new UserNotFoundException(request.UserId);

        return await auction.MatchAsync(
            a => CreateEntity(request, a, cancellationToken),
            () => new AuctionNotFoundException(request.AuctionId));
    }

    private async Task<Either<BaseException, Bid>> CreateEntity(
        CreateBidCommand request, Auction auction, CancellationToken cancellationToken)
    {
        try
        {
            if (!auction.IsActive())
                return new AuctionNotActiveException(auction.Id);

            if (auction.OwnerId == request.UserId)
                return new BidOnOwnAuctionException(auction.Id);

            if (request.Amount <= auction.CurrentBid)
                return new BidTooLowException(auction.Id);

            var bid = await bidRepository.CreateAsync(
                Bid.New(request.AuctionId, request.UserId, request.Amount),
                cancellationToken);

            auction.UpdateCurrentBid(request.Amount);
            await auctionRepository.UpdateAsync(auction, cancellationToken);

            return bid;
        }
        catch (Exception ex)
        {
            return new UnhandledBidException(0, ex);
        }
    }
}