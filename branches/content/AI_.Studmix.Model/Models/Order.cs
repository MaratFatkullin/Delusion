using AI_.Data;

namespace AI_.Studmix.Model.Models
{
    public class Order : ModelBase
    {
        public virtual ContentPackage ContentPackage { get; set; }

        public virtual UserProfile UserProfile { get; set; }
    }
}