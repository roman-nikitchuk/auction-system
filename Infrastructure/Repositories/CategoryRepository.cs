using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, IRepository<Category>, ICategoryQueries
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken)
    {
        await _context.Categories.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Category> UpdateAsync(Category entity, CancellationToken cancellationToken)
    {
        _context.Categories.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Category> DeleteAsync(Category entity, CancellationToken cancellationToken)
    {
        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Option<Category>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var entity = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);

        return entity ?? Option<Category>.None;
    }
}