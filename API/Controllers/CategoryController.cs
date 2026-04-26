using Api.Dtos;
using Application.Common.Interfaces.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController(ICategoryQueries categoryQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var categories = await categoryQueries.GetAllAsync(cancellationToken);
        return Ok(categories.Select(CategoryDto.FromDomainModel).ToList());
    }

    [HttpGet("{categoryId:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(
        [FromRoute] int categoryId,
        CancellationToken cancellationToken)
    {
        var result = await categoryQueries.GetByIdAsync(categoryId, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            c => Ok(CategoryDto.FromDomainModel(c)),
            () => NotFound());
    }
}