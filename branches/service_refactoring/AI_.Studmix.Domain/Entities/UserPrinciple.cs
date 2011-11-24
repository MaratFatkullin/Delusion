using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AI_.Data;

namespace AI_.Studmix.Domain.Entities
{
    public class UserPrinciple : Entity
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public string PasswordQuestion { get; set; }

        public string PasswordAnswer { get; set; }

        public bool IsApproved { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? LastActivityDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? LastLockoutDate { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }

        public virtual ICollection<Role> Roles { get; protected set; }

        public UserPrinciple()
        {
            Roles = new Collection<Role>();
        }

        public bool IsInRole(string role)
        {
            return Roles.Any(r=>r.RoleName == role);
        }

    }
}