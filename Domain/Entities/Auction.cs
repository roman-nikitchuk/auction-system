using Domain.Interfaces;

namespace Domain.Entities
{
    public class Auction : AuditableEntity, IEntity
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int CategoryId { get; private set; }
        public AuctionStatus Status { get; private set; }
        public decimal StartingPrice { get; private set; }
        public decimal CurrentBid { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public User? Owner { get; private set; }
        public Category? Category { get; private set; }
        public ICollection<Bid>? Bids { get; private set; } = [];

        private Auction(
            int userId, string title, string description, int categoryId,
            decimal startingPrice, DateTime startDate, DateTime endDate)
        {
            UserId = userId;
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Status = AuctionStatus.Active;
            StartingPrice = startingPrice;
            CurrentBid = startingPrice;
            StartDate = startDate;
            EndDate = endDate;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public static Auction New(
            int userId, string title, string description, int categoryId,
            decimal startingPrice, DateTime startDate, DateTime endDate)
            => new Auction(userId, title, description, categoryId,
                startingPrice, startDate, endDate);

        public void UpdateDetails(
            string title, string description, int categoryId,
            DateTime startDate, DateTime endDate)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            StartDate = startDate;
            EndDate = endDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCurrentBid(decimal newBid)
        {
            CurrentBid = newBid;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Close()
        {
            Status = AuctionStatus.Finished;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsActive() => Status == AuctionStatus.Active && EndDate > DateTime.UtcNow;
    }
}