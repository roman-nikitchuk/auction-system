using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using MediatR;

namespace Application.Users.Commands;

public class UpdateUserCommand : IRequest<Either<BaseException, User>>
{
    public required int Id { get; init; }
    public required string UserName { get; init; }
    public required string Email { get; init; }
}

public class UpdateUserCommandHandler(
    IRepository<User> userRepository,
    IUserQueries userQueries) : IRequestHandler<UpdateUserCommand, Either<BaseException, User>>
{
    public async Task<Either<BaseException, User>> Handle(
        UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userQueries.GetByIdAsync(request.Id, cancellationToken);

        return await user.MatchAsync(
            u => UpdateEntity(request, u, cancellationToken),
            () => new UserNotFoundException(request.Id));
    }

    private async Task<Either<BaseException, User>> UpdateEntity(
        UpdateUserCommand request, User user, CancellationToken cancellationToken)
    {
        try
        {
            user.UpdateDetails(request.UserName, request.Email);
            return await userRepository.UpdateAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            return new UnhandledUserException(request.Id, ex);
        }
    }
}