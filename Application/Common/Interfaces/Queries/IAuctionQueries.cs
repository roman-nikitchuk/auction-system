using Domain;
using Domain.Entities;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IAuctionQueries : IBaseQuery<Auction>
{
    Task<IReadOnlyList<Auction>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Auction>> GetByStatusAsync(AuctionStatus status, CancellationToken cancellationToken);
    Task<IReadOnlyList<Auction>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken);
}