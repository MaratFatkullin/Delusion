using System;
using System.Linq;
using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services.Abstractions;

namespace AI_.Studmix.Model.Services
{
    public class MembershipService : ServiceBase
    {
        public MembershipService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public UserProfile GetUserProfile(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
           
            return UnitOfWork.UserProfileRepository
                .Get(profile=>profile.User.ID == user.ID)
                .Single();
        }

        public User GetUser(string username)
        {
            if (username == null)
                throw new ArgumentNullException("username");
           
            return UnitOfWork.UserRepository
                .Get(user => user.UserName == username)
                .Single();
        }
    }
}