using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using Unit = LanguageExt.Unit;
using LanguageExt;
using MediatR;

namespace Application.Auctions.Commands;

public class UpdateAuctionCommand : IRequest<Either<BaseException, Auction>>
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int CategoryId { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
}

public class UpdateAuctionCommandHandler(
    IRepository<Auction> auctionRepository,
    IAuctionQueries auctionQueries,
    ICategoryQueries categoryQueries) : IRequestHandler<UpdateAuctionCommand, Either<BaseException, Auction>>
{
    public async Task<Either<BaseException, Auction>> Handle(
        UpdateAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = await auctionQueries.GetByIdAsync(request.Id, cancellationToken);

        return await auction.MatchAsync(
            async a =>
            {
                if (!a.IsActive())
                    return (Either<BaseException, Auction>)new AuctionNotActiveException(a.Id);

                return await CheckDependencies(request, cancellationToken)
                    .BindAsync(_ => UpdateEntity(request, a, cancellationToken));
            },
            () => new AuctionNotFoundException(request.Id));
    }

    private async Task<Either<BaseException, Unit>> CheckDependencies(
        UpdateAuctionCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryQueries.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category.IsNone)
            return new CategoryNotFoundException(request.CategoryId);

        return Unit.Default;
    }

    private async Task<Either<BaseException, Auction>> UpdateEntity(
        UpdateAuctionCommand request, Auction auction, CancellationToken cancellationToken)
    {
        try
        {
            auction.UpdateDetails(
                request.Title,
                request.Description,
                request.CategoryId,
                request.StartDate,
                request.EndDate);

            return await auctionRepository.UpdateAsync(auction, cancellationToken);
        }
        catch (Exception ex)
        {
            return new UnhandledAuctionException(request.Id, ex);
        }
    }
}