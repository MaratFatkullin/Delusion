using System;
using System.Linq;
using AI_.Data.Repository;
using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services
{
    public class MembershipService : Security.Services.MembershipService
    {
        public MembershipService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public UserProfile GetUserProfile(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            var unitOfWork = (IUnitOfWork) UnitOfWork;
            return unitOfWork.GetRepository<UserProfile>()
                .Get(profile => profile.User.ID == user.ID)
                .Single();
        }
    }
}