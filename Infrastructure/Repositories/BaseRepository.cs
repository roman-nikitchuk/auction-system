using Application.Common.Interfaces.Queries;
using Domain.Interfaces;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class BaseRepository<T> : IBaseQuery<T> where T : class, IEntity
{
    private readonly AppDbContext _context;

    protected BaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<Option<T>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? Option<T>.None;
    }
}