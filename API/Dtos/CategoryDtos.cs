using Domain.Entities;

namespace Api.Dtos;

public record CategoryDto(
    int Id,
    string Name)
{
    public static CategoryDto FromDomainModel(Category model)
        => new(model.Id, model.Name);
}

public record CreateCategoryDto(string Name);