using System;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.ApplicationServices.Tests.UtilityClasses
{
    public class UtilityMethods
    {
        public static User CreateUser(string username = "username",
                                      string password = "password",
                                      string email = "email",
                                      string passwordQuestion = "passwordQuestion",
                                      string passwordAnswer = "passwordAnswer",
                                      bool isApproved = true)
        {
            var userPrincipal = new UserPrinciple
                                {
                                    UserName = username,
                                    Password = password,
                                    Email = email,
                                    PasswordQuestion = passwordQuestion,
                                    PasswordAnswer = passwordAnswer,
                                    IsApproved = isApproved,
                                    IsLocked = false,
                                    LastPasswordChangedDate = DateTime.Today,
                                    LastActivityDate = DateTime.Today,
                                    LastLockoutDate = DateTime.MinValue.ToLocalTime(),
                                    LastLoginDate = DateTime.MinValue.ToLocalTime(),
                                };

            var user = new User(){UserPrinciple = userPrincipal};
            return user;
        }
    }
}