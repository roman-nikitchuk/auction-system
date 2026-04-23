using Domain.Entities;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IUserQueries : IBaseQuery<User>
{
    Task<Option<User>> GetByEmailAsync(string email, CancellationToken cancellationToken);
}