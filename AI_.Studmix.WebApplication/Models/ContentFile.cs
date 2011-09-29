using AI_.Data;

namespace AI_.Studmix.WebApplication.Models
{
    public class ContentFile: ModelBase
    {
        public virtual ContentPackage ContentPackage { get; set; }

        public string Name { get; set; }
    }
}