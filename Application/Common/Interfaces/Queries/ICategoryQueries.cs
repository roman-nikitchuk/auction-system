using CategoryEntity = Domain.Entities.Category;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface ICategoryQueries : IBaseQuery<CategoryEntity>
{
    Task<Option<CategoryEntity>> GetByNameAsync(string name, CancellationToken cancellationToken);
}