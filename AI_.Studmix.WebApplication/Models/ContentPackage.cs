using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AI_.Data;
using AI_.Security.Models;

namespace AI_.Studmix.WebApplication.Models
{
    public class ContentPackage : ModelBase
    {
        public virtual ICollection<PropertyState> PropertyStates { get; set; }

        public virtual User Owner { get; set; }

        public virtual ICollection<ContentFile> Files { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public int Price { get; set; }
    }
}