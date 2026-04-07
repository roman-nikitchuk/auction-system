
namespace Domain.Interfaces
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
    }
}