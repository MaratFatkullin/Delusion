using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AI_.Data;
using AI_.Security.Models;

namespace AI_.Studmix.Model.Models
{
    public class UserProfile : ModelBase
    {
        public User User { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        [Range(0,int.MaxValue,ErrorMessage="Баланс не может быть отрицательным.")]
        public int Balance { get; set; }

        public string PhoneNumber { get; set; }
    }
}