using System.Collections.Generic;
using AI_.Data;
using AI_.Security.Models;

namespace AI_.Studmix.Model.Models
{
    public class UserProfile : ModelBase
    {
        public User User { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}