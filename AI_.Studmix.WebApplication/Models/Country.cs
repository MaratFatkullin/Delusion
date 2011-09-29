using System.Collections.Generic;
using AI_.Data;

namespace AI_.Studmix.WebApplication.Models
{
    public class Country : ModelBase
    {
        public string Name { get; set; }

        public virtual ICollection<City> Cities { get; set; }
    }
}