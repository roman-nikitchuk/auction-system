using Api.Dtos;
using Api.Modules.Errors;
using Application.Categories.Commands;
using Application.Common.Interfaces.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoryController(
    ISender sender,
    ICategoryQueries categoryQueries) : ControllerBase
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

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand
        {
            Name = request.Name
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<CategoryDto>>(
            c => CreatedAtAction(nameof(GetById),
                new { categoryId = c.Id },
                CategoryDto.FromDomainModel(c)),
            e => e.ToObjectResult());
    }
}