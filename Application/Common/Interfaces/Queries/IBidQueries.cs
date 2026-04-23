using Domain.Entities;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IBidQueries 
{
    Task<IReadOnlyList<Bid>> GetByAuctionIdAsync(int auctionId, CancellationToken cancellationToken);
    Task<Option<Bid>> GetHighestBidAsync(int auctionId, CancellationToken cancellationToken);
}