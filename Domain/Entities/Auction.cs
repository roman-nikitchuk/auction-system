using Domain.Interfaces;

namespace Domain.Entities
{
    public class Auction : AuditableEntity, IEntity
    {
        public int Id { get; private set; }
        public int OwnerId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int CategoryId { get; private set; }
        public AuctionStatus Status { get; private set; }
        public decimal StartingPrice { get; private set; }
        public decimal CurrentBid { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public string? ImageUrl { get; private set; }
        public User? Owner { get; private set; }
        public Category? Category { get; private set; }
        public ICollection<Bid>? Bids { get; private set; } = [];

        private Auction(
            int ownerId, string title, string description, int categoryId,
            decimal startingPrice, DateTime startDate, DateTime endDate,
            string? imageUrl = null)
        {
            OwnerId = ownerId;
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Status = AuctionStatus.Active;
            StartingPrice = startingPrice;
            CurrentBid = startingPrice;
            StartDate = startDate;
            EndDate = endDate;
            ImageUrl = imageUrl;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public static Auction New(
            int ownerId, string title, string description, int categoryId,
            decimal startingPrice, DateTime startDate, DateTime endDate,
            string? imageUrl = null)
            => new Auction(ownerId, title, description, categoryId,
                startingPrice, startDate, endDate, imageUrl);

        public void UpdateDetails(
            string title, string description, int categoryId,
            DateTime startDate, DateTime endDate,
            string? imageUrl = null)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            StartDate = startDate;
            EndDate = endDate;
            ImageUrl = imageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCurrentBid(decimal newBid)
        {
            CurrentBid = newBid;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Close()
        {
            Status = AuctionStatus.Ended;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsActive() => Status == AuctionStatus.Active && EndDate > DateTime.UtcNow;
    }
}