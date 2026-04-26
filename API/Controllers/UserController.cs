using Api.Dtos;
using Api.Modules.Errors;
using Application.Users.Commands;
using Application.Common.Interfaces.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(ISender sender, IUserQueries userQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var users = await userQueries.GetAllAsync(cancellationToken);
        return Ok(users.Select(UserDto.FromDomainModel).ToList());
    }

    [HttpGet("{userId:int}")]
    public async Task<ActionResult<UserDto>> GetById(
        [FromRoute] int userId,
        CancellationToken cancellationToken)
    {
        var result = await userQueries.GetByIdAsync(userId, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => Ok(UserDto.FromDomainModel(u)),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(
        [FromBody] CreateUserDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand
        {
            UserName = request.UserName,
            Email = request.Email,
            Password = request.Password
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => CreatedAtAction(nameof(GetById),
                new { userId = u.Id },
                UserDto.FromDomainModel(u)),
            e => e.ToObjectResult());
    }

    [HttpPut("{userId:int}")]
    public async Task<ActionResult<UserDto>> Update(
        [FromRoute] int userId,
        [FromBody] UpdateUserDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand
        {
            Id = userId,
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<UserDto>>(
            u => Ok(UserDto.FromDomainModel(u)),
            e => e.ToObjectResult());
    }

    [HttpDelete("{userId:int}")]
    public async Task<ActionResult> Delete(
        [FromRoute] int userId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand { Id = userId };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}