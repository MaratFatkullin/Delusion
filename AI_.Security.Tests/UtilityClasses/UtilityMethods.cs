using System;
using System.Collections.ObjectModel;
using AI_.Security.Models;

namespace AI_.Security.Tests.UtilityClasses
{
    public class UtilityMethods
    {
        public static User CreateUser(string username = "username",
                             string password = "password",
                             string email = "email",
                             string passwordQuestion = "passwordQuestion",
                             string passwordAnswer = "passwordAnswer",
                             bool isApproved = true,
                             int id = 0)
        {
            var user = new User
                       {
                           ID = id,
                           UserName = username,
                           Password = password,
                           Email = email,
                           PasswordQuestion = passwordQuestion,
                           PasswordAnswer = passwordAnswer,
                           IsApproved = isApproved,
                           IsLocked = false,
                           LastPasswordChangedDate = DateTime.Today,
                           CreateDate = DateTime.Today,
                           LastActivityDate = DateTime.Today,
                           LastLockoutDate = DateTime.MinValue.ToLocalTime(),
                           LastLoginDate = DateTime.MinValue.ToLocalTime(),
                           Roles = new Collection<Role>()
                       };

            return user;
        }

        public static Role CreateRole(string roleName = "roleName")
        {
            return new Role
                   {
                       RoleName = roleName,
                       Users = new Collection<User>()
                   };
        }
    }
}