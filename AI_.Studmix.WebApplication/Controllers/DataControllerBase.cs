using System;
using System.Linq;
using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
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

                return _currentUser ?? (_currentUser = new MembershipService(UnitOfWork).GetUser(User.Identity.Name));
            }
        }

        protected UserProfile CurrentUserProfile
        {
            get
            {
                if(_currentUserProfile == null)
                    _currentUserProfile = new MembershipService(UnitOfWork).GetUserProfile(CurrentUser);
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