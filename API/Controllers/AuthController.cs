using Api.Dtos;
using Api.Modules.Errors;
using Application.Users.Commands;
using Application.Users.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(
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
            u => CreatedAtAction(nameof(Register),
                new { userId = u.Id },
                UserDto.FromDomainModel(u)),
            e => e.ToObjectResult());
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(
        [FromBody] LoginUserDto request,
        CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<string>>(
            token => Ok(new { token }),
            e => e.ToObjectResult());
    }
}