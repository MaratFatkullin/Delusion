using AI_.Data;

namespace AI_.Studmix.Model.Models
{
    public class Purchase : ModelBase
    {
        public virtual ContentPackage ContentPackage { get; set; }

        public virtual UserProfile User { get; set; }
    }
}