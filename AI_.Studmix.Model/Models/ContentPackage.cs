using System.Collections.Generic;
using AI_.Data;
using AI_.Security.Models;
using AI_.Studmix.Model.Validation;

namespace AI_.Studmix.Model.Models
{
    public class ContentPackage : ModelBase
    {
        public virtual ICollection<PropertyState> PropertyStates { get; set; }

        public virtual User Owner { get; set; }

        [NotEmpty]
        public virtual ICollection<ContentFile> Files { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }
    }
}