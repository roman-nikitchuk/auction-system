using Domain.Entities;

namespace Application.Common.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}