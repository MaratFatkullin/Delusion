using System.ComponentModel.DataAnnotations;
using System.IO;
using AI_.Data;

namespace AI_.Studmix.Model.Models
{
    public class ContentFile: ModelBase
    {
        public virtual ContentPackage ContentPackage { get; set; }

        public string Name { get; set; }

        public bool IsPreview { get; set; }

        [NotMapped]
        public Stream Stream { get; set; }
    }
}