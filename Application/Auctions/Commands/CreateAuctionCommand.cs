using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using MediatR;

namespace Application.Auctions.Commands;

public class CreateAuctionCommand : IRequest<Either<BaseException, Auction>>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int CategoryId { get; init; }
    public required decimal StartingPrice { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required int OwnerId { get; init; }
}

public class CreateAuctionCommandHandler(
    IRepository<Auction> auctionRepository,
    IUserQueries userQueries,
    ICategoryQueries categoryQueries) : IRequestHandler<CreateAuctionCommand, Either<BaseException, Auction>>
{
    public async Task<Either<BaseException, Auction>> Handle(
        CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        var owner = await userQueries.GetByIdAsync(request.OwnerId, cancellationToken);
        if (owner.IsNone)
            return new UserNotFoundException(request.OwnerId);

        var category = await categoryQueries.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category.IsNone)
            return new CategoryNotFoundException(request.CategoryId);

        return await CreateEntity(request, cancellationToken);
    }

    private async Task<Either<BaseException, Auction>> CreateEntity(
        CreateAuctionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var auction = await auctionRepository.CreateAsync(
                Auction.New(
                    request.OwnerId,
                    request.Title,
                    request.Description,
                    request.CategoryId,
                    request.StartingPrice,
                    request.StartDate,
                    request.EndDate),
                cancellationToken);

            return auction;
        }
        catch (Exception ex)
        {
            return new UnhandledAuctionException(0, ex);
        }
    }
}