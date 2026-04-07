using Domain.Interfaces;

namespace Domain.Entities
{
    public class Role : IEntity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        private Role(string name)
        {
            Name = name;
        }

        public static Role New(string name)
            => new Role(name);
    }
}