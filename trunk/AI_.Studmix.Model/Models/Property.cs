using System.Collections.Generic;
using System.Linq;
using AI_.Data;
using AI_.Studmix.Model.DAL.Database;

namespace AI_.Studmix.Model.Models
{
    public class Property : ModelBase
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public virtual ICollection<PropertyState> States { get; set; }
    }
}