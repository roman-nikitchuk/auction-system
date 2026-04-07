using Domain.Interfaces;

namespace Domain.Entities
{
    public class Category : IEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public ICollection<Auction>? Auctions { get; private set; } = [];

        private Category(string name)
        {
            Name = name;
        }

        public static Category New(string name)
            => new Category(name);
    }
}