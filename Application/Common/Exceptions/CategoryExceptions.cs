namespace Application.Common.Exceptions;

public class CategoryNotFoundException(int categoryId)
    : BaseException(categoryId, $"Category not found under id {categoryId}");

public class CategoryAlreadyExistsException(int categoryId)
    : BaseException(categoryId, $"Category with id {categoryId} already exists");

public class UnhandledCategoryException(int categoryId, Exception? innerException = null)
    : BaseException(categoryId, "Unhandled category exception", innerException);