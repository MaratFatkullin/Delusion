using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AI_.Data;

namespace AI_.Studmix.Domain.Entities
{
    public class User : AggregationRoot
    {
        public UserPrinciple UserPrinciple { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        [Range(0,int.MaxValue,ErrorMessage="Баланс не может быть отрицательным.")]
        public int Balance { get; set; }

        public string PhoneNumber { get; set; }
    }
}