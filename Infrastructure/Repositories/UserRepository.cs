using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IRepository<User>, IUserQueries
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public override async Task<Option<User>> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity ?? Option<User>.None;
    }

    public async Task<User> CreateAsync(User entity, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<User> UpdateAsync(User entity, CancellationToken cancellationToken)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<User> DeleteAsync(User entity, CancellationToken cancellationToken)
    {
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Option<User>> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

        return entity ?? Option<User>.None;
    }
}