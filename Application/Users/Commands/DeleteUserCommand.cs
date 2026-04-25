using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using MediatR;

namespace Application.Users.Commands;

public class DeleteUserCommand : IRequest<Either<BaseException, User>>
{
    public required int Id { get; init; }
}

public class DeleteUserCommandHandler(
    IRepository<User> userRepository,
    IUserQueries userQueries) : IRequestHandler<DeleteUserCommand, Either<BaseException, User>>
{
    public async Task<Either<BaseException, User>> Handle(
        DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userQueries.GetByIdAsync(request.Id, cancellationToken);

        return await user.MatchAsync(
            u => DeleteEntity(u, cancellationToken),
            () => new UserNotFoundException(request.Id));
    }

    private async Task<Either<BaseException, User>> DeleteEntity(
        User user, CancellationToken cancellationToken)
    {
        try
        {
            return await userRepository.DeleteAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            return new UnhandledUserException(user.Id, ex);
        }
    }
}