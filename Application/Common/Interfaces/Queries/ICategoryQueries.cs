using Domain.Entities;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface ICategoryQueries : IBaseQuery<Category>
{
    Task<Option<Category>> GetByNameAsync(string name, CancellationToken cancellationToken);
}