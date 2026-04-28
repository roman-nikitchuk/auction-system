using Application.Common.Exceptions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using LanguageExt;
using MediatR;
using CategoryEntity = Domain.Entities.Category;

namespace Application.Categories.Commands;

public class CreateCategoryCommand : IRequest<Either<BaseException, CategoryEntity>>
{
    public required string Name { get; init; }
}

public class CreateCategoryCommandHandler(
    IRepository<CategoryEntity> categoryRepository,
    ICategoryQueries categoryQueries) : IRequestHandler<CreateCategoryCommand, Either<BaseException, CategoryEntity>>
{
    public async Task<Either<BaseException, CategoryEntity>> Handle(
        CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existing = await categoryQueries.GetByNameAsync(request.Name, cancellationToken);

        return await existing.MatchAsync(
            c => (Either<BaseException, CategoryEntity>)new CategoryAlreadyExistsException(c.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<BaseException, CategoryEntity>> CreateEntity(
        CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await categoryRepository.CreateAsync(
                CategoryEntity.New(request.Name),
                cancellationToken);

            return category;
        }
        catch (Exception ex)
        {
            return new UnhandledCategoryException(0, ex);
        }
    }
}