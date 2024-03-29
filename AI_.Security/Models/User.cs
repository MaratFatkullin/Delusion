﻿using System;
using System.Collections.Generic;
using AI_.Data;

namespace AI_.Security.Models
{
    public class User : ModelBase
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

        public virtual ICollection<Role> Roles { get; set; }
    }
}