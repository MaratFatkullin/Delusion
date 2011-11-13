using System;
using System.Linq;
using AI_.Data.Repository;
using AI_.Security.Models;
using AI_.Security.Services;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services.Abstractions;

namespace AI_.Studmix.Model.Services
{
    public class ProfileService : ServiceBase, IProfileService
    {
        public ProfileService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public UserProfile GetUserProfile(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return UnitOfWork.GetRepository<UserProfile>()
                .Get(profile => profile.User.ID == user.ID)
                .Single();
        }

        public UserProfile GetUserProfile(int userId)
        {
            return UnitOfWork.GetRepository<UserProfile>()
                .Get(p => p.User.ID == userId).Single();
        }

        public void CreateUserProfile(User user, string phoneNumber)
        {
            var profile = new UserProfile
                              {
                                  User = user,
                                  Balance = 0,
                                  PhoneNumber = phoneNumber
                              };
            UnitOfWork.GetRepository<UserProfile>().Insert(profile);
            UnitOfWork.Save();
        }
    }
}