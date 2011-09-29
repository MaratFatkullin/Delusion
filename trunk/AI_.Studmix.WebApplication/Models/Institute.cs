using System.Collections.Generic;
using AI_.Data;

namespace AI_.Studmix.WebApplication.Models
{
    public class Institute : ModelBase
    {
        public string Name { get; set; }
        public virtual InstituteType Type { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Faculty> Faculties { get; set; }
    }
}