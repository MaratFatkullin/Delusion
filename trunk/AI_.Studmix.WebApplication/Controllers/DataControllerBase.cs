using System;
using AI_.Data.Repository;
using AI_.Security.Models;
using AI_.Security.Services;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;

namespace AI_.Studmix.WebApplication.Controllers
{
    public abstract class DataControllerBase : ControllerBase
    {
        private User _currentUser;
        private UserProfile _currentUserProfile;

        protected IUnitOfWork UnitOfWork { get; private set; }

        protected User CurrentUser
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    throw new InvalidOperationException("User is not authenticated.");

                if (_currentUser == null)
                {
                    var membershipService = new MembershipService(UnitOfWork);
                    _currentUser = membershipService.GetUser(User.Identity.Name);
                }

                return _currentUser;
            }
        }

        protected UserProfile CurrentUserProfile
        {
            get
            {
                if (_currentUserProfile == null)
                {
                    var profileService = new ProfileService(UnitOfWork);
                    _currentUserProfile = profileService.GetUserProfile(CurrentUser);
                }
                return _currentUserProfile;
            }
        }

        protected DataControllerBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            UnitOfWork.Dispose();
        }
    }
}