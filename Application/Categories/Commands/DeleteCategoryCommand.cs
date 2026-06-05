using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Entities;
using LanguageExt;
using MediatR;

namespace Application.Categories.Commands;

public class DeleteCategoryCommand : IRequest<Either<BaseException, Category>>
{
    public required int Id { get; init; }
}

public class DeleteCategoryCommandHandler(
    IRepository<Category> categoryRepository,
    ICategoryQueries categoryQueries)
    : IRequestHandler<DeleteCategoryCommand, Either<BaseException, Category>>
{
    public async Task<Either<BaseException, Category>> Handle(
    DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryQueries.GetByIdAsync(request.Id, cancellationToken);

        if (category.IsNone)
            return new CategoryNotFoundException(request.Id);

        return await category.MatchAsync(
            async c =>
            {
                try
                {
                    return (Either<BaseException, Category>)await categoryRepository.DeleteAsync(c, cancellationToken);
                }
                catch (Exception ex)
                {
                    return (Either<BaseException, Category>)new UnhandledCategoryException(c.Id, ex);
                }
            },
            () => new CategoryNotFoundException(request.Id));
    }
}