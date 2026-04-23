using Domain.Interfaces;

namespace Domain.Entities
{
    public class User : AuditableEntity, IEntity
    {
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public UserRole Role { get; private set; }

        public ICollection<Auction>? Auctions { get; private set; } = [];
        public ICollection<Bid>? Bids { get; private set; } = [];

        private User(
            string userName, string email, string passwordHash, UserRole role = UserRole.User)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public static User New(string userName, string email, string passwordHash, UserRole role = UserRole.User)
            => new User(userName, email, passwordHash, role);

        public void UpdateDetails(string userName, string email)
        {
            UserName = userName;
            Email = email;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}