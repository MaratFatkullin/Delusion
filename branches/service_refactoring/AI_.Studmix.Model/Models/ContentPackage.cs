using System.Collections.Generic;
using System.ComponentModel;
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

        [DisplayName("Название")]
        public string Caption { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; }

        [DisplayName("Цена")]
        public int Price { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public string Path { get; set; }
    }
}