using System.ComponentModel.DataAnnotations;
using System.IO;
using AI_.Data;

namespace AI_.Studmix.Domain.Entities
{
    public class ContentFile: Entity
    {
        public virtual ContentPackage ContentPackage { get; set; }

        public string Name { get; set; }

        public bool IsPreview { get; set; }

        [NotMapped]
        public Stream Stream { get; set; }
    }
}