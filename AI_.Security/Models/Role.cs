using System.Collections.Generic;
using AI_.Data;

namespace AI_.Security.Models
{
    public class Role : ModelBase
    {
        public string RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}