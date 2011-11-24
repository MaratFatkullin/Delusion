using AI_.Data;

namespace AI_.Studmix.Domain.Entities
{
    public class Order : Entity
    {
        public Order(User user, ContentPackage package)
        {
            User = user;
            ContentPackage = package;
        }

        public ContentPackage ContentPackage { get; set; }

        public User User { get; set; }
    }
}