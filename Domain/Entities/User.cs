using Domain.Interfaces;

namespace Domain.Entities
{
    public class User : AuditableEntity, IEntity
    {
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public int RoleId { get; private set; }

        public Role? Role { get; private set; }
        public ICollection<Auction>? Auctions { get; private set; } = [];
        public ICollection<Bid>? Bids { get; private set; } = [];

        private User(
            string userName, string email, string passwordHash, int roleId)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            RoleId = roleId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
        }

        public static User New(string userName, string email, string passwordHash, int roleId)
            => new User(userName, email, passwordHash, roleId);

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