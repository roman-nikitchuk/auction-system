namespace Application.Common.Exceptions;

public abstract class BaseException(int id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public int Id { get; } = id;
}