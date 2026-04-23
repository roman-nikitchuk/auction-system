using Domain.Interfaces;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IBaseQuery<T> where T : class, IEntity
{
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<Option<T>> GetByIdAsync(int id, CancellationToken cancellationToken);
}