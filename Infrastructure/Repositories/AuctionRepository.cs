using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain;
using Domain.Entities;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AuctionRepository : BaseRepository<Auction>, IRepository<Auction>, IAuctionQueries
{
    private readonly AppDbContext _context;

    public AuctionRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<IReadOnlyList<Auction>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Auctions
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public override async Task<Option<Auction>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Auctions
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .Include(x => x.Bids)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? Option<Auction>.None;
    }

    public async Task<Auction> CreateAsync(Auction entity, CancellationToken cancellationToken)
    {
        await _context.Auctions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Auction> UpdateAsync(Auction entity, CancellationToken cancellationToken)
    {
        _context.Auctions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Auction> DeleteAsync(Auction entity, CancellationToken cancellationToken)
    {
        _context.Auctions.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IReadOnlyList<Auction>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        return await _context.Auctions
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(a => a.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Auction>> GetByStatusAsync(AuctionStatus status, CancellationToken cancellationToken)
    {
        return await _context.Auctions
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(a => a.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Auction>> GetByOwnerIdAsync(int ownerId, CancellationToken cancellationToken)
    {
        return await _context.Auctions
            .Include(x => x.Owner)
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(a => a.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }
}