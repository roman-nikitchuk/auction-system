using Domain.Interfaces;

namespace Domain.Entities
{
    public class Bid : IEntity
    {
        public int Id { get; private set; }
        public int AuctionId { get; private set; }
        public int UserId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Auction? Auction { get; private set; }
        public User? User { get; private set; }

        private Bid(int auctionId, int userId, decimal amount)
        {
            AuctionId = auctionId;
            UserId = userId;
            Amount = amount;
            CreatedAt = DateTime.UtcNow;
        }

        public static Bid New(int auctionId, int userId, decimal amount)
            => new Bid(auctionId, userId, amount);
    }
}