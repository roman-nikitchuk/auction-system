using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Services;
using Domain.Entities;
using LanguageExt;
using MediatR;

namespace Application.Users.Commands.Auth;
public class LoginUserCommand : IRequest<Either<BaseException, string>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginUserCommandHandler(
    IUserQueries userQueries,
    ITokenService tokenService) : IRequestHandler<LoginUserCommand, Either<BaseException, string>>
{
    public async Task<Either<BaseException, string>> Handle(
        LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userQueries.GetByEmailAsync(request.Email, cancellationToken);

        return user.Match<Either<BaseException, string>>(
            u => VerifyAndGenerateToken(u, request.Password),
            () => new UserNotFoundException(request.Email));
    }

    private Either<BaseException, string> VerifyAndGenerateToken(User user, string password)
    {
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return new UserNotFoundException(user.Email);

        return tokenService.GenerateToken(user);
    }
}
