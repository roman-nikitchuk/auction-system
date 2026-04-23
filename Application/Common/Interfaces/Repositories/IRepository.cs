using Domain.Interfaces;

namespace Application.Common.Interfaces.Repositories;

public interface IRepository<T> where T : class, IEntity
{
    Task<T> CreateAsync(T entity, CancellationToken cancellationToken);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<T> DeleteAsync(T entity, CancellationToken cancellationToken);
}