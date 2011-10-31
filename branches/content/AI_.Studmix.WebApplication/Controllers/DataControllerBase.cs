using System;
using System.Linq;
using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;

namespace AI_.Studmix.WebApplication.Controllers
{
    public abstract class DataControllerBase : ControllerBase
    {
        private User _currentUser;

        protected IUnitOfWork UnitOfWork { get; private set; }

        protected User CurrentUser
        {
            get
            {
                if (!User.Identity.IsAuthenticated)
                    throw new InvalidOperationException("User is not authenticated.");

                return _currentUser ?? (_currentUser = UnitOfWork.UserRepository
                                                           .Get(user => user.UserName == User.Identity.Name)
                                                           .Single());
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