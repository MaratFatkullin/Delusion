using System.Collections.Generic;
using AI_.Data;
using AI_.Security.Models;

namespace AI_.Studmix.WebApplication.Models
{
    public class ContentPackage : ModelBase
    {
        public virtual Faculty Faculty { get; set; }
        public virtual Institute Institute { get; set; }
        public virtual StudingForm StudingForm { get; set; }
        public virtual Course Course { get; set; }
        public virtual Group Group { get; set; }
        public virtual User Owner { get; set; }
        public virtual ICollection<ContentFile> Files { get; set; }
    }
}