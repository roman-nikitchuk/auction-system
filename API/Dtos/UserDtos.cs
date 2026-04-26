using Domain.Entities;

namespace Api.Dtos;

public record UserDto(
    int Id,
    string UserName,
    string Email,
    string Role,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    public static UserDto FromDomainModel(User model)
        => new(
            model.Id,
            model.UserName,
            model.Email,
            model.Role.ToString(),
            model.CreatedAt,
            model.UpdatedAt);
}

public record CreateUserDto(
    string UserName,
    string Email,
    string Password);

public record UpdateUserDto(
    string UserName,
    string Email);