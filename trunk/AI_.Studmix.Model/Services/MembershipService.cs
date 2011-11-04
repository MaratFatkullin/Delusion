using System;
using System.Linq;
using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services
{
    public class MembershipService
    {
        public UserProfile GetUserProfile(IUnitOfWork unitOfWork, User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            return unitOfWork.UserProfileRepository
                .Get(profile=>profile.User.ID == user.ID)
                .Single();
        }

        public User GetUser(IUnitOfWork unitOfWork, string username)
        {
            if (username == null)
                throw new ArgumentNullException("username");
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            return unitOfWork.UserRepository
                .Get(user => user.UserName == username)
                .Single();
        }
    }
}