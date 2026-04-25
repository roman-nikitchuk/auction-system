using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BidRepository : IRepository<Bid>, IBidQueries
{
    private readonly AppDbContext _context;

    public BidRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Bid> CreateAsync(Bid entity, CancellationToken cancellationToken)
    {
        await _context.Bids.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Bid> UpdateAsync(Bid entity, CancellationToken cancellationToken)
    {
        _context.Bids.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Bid> DeleteAsync(Bid entity, CancellationToken cancellationToken)
    {
        _context.Bids.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<Bid>> GetByAuctionIdAsync(int auctionId, CancellationToken cancellationToken)
    {
        return await _context.Bids
            .Include(x => x.User)
            .AsNoTracking()
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.Amount)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Bid>> GetHighestBidAsync(int auctionId, CancellationToken cancellationToken)
    {
        var entity = await _context.Bids
            .AsNoTracking()
            .Where(b => b.AuctionId == auctionId)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync(cancellationToken);

        return entity ?? Option<Bid>.None;
    }
}