using System.Collections.Generic;
using System.Linq;
using AI_.Data;
using AI_.Studmix.Model.DAL.Database;

namespace AI_.Studmix.Model.Models
{
    public class PropertyState : ModelBase
    {
        public virtual Property Property { get; set; }

        public virtual ICollection<ContentPackage> ContentPackages { get; set; }

        public string Value { get; set; }

        public int Index { get; set; }
    }
}