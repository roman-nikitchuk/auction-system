using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using MediatR;

namespace Application.Auctions.Commands;

public class DeleteAuctionCommand : IRequest<Either<BaseException, Auction>>
{
    public required int Id { get; init; }
}

public class DeleteAuctionCommandHandler(
    IRepository<Auction> auctionRepository,
    IAuctionQueries auctionQueries) : IRequestHandler<DeleteAuctionCommand, Either<BaseException, Auction>>
{
    public async Task<Either<BaseException, Auction>> Handle(
        DeleteAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = await auctionQueries.GetByIdAsync(request.Id, cancellationToken);

        return await auction.MatchAsync(
            a => DeleteEntity(a, cancellationToken),
            () => new AuctionNotFoundException(request.Id));
    }

    private async Task<Either<BaseException, Auction>> DeleteEntity(
        Auction auction, CancellationToken cancellationToken)
    {
        try
        {
            return await auctionRepository.DeleteAsync(auction, cancellationToken);
        }
        catch (Exception ex)
        {
            return new UnhandledAuctionException(auction.Id, ex);
        }
    }
}