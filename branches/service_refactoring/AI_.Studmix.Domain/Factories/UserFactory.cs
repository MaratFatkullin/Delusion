using System.Collections.ObjectModel;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.Domain.Factories
{
    public class UserFactory 
    {
        public User CreateUser(string username,
                               string password,
                               string email,
                               string phoneNumber,
                               Role role)
        {
            var user = new User
                       {
                           Balance = 0,
                           PhoneNumber = phoneNumber,
                           Orders = new Collection<Order>(),
                           UserPrinciple = CreateUserPrinciple(username, password, email, role)
                       };

            return user;
        }

        protected UserPrinciple CreateUserPrinciple(string username, string password, string email, Role role)
        {
            var userPrinciple = new UserPrinciple
                                {
                                    Email = email,
                                    IsApproved = true,
                                    IsLocked = false,
                                    UserName = username,
                                    Password = password,
                                    PasswordAnswer = null,
                                    PasswordQuestion = null
                                };
            userPrinciple.Roles.Add(role);

            return userPrinciple;
        }
    }
}