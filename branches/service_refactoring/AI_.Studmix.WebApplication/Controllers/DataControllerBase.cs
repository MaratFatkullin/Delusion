using System;
using AI_.Data.Repository;
using AI_.Studmix.ApplicationServices.Services.Abstractions;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.WebApplication.Infrastructure;

namespace AI_.Studmix.WebApplication.Controllers
{
    public abstract class DataControllerBase : ControllerBase
    {

        //protected override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);
        //    var username = User.Identity.Name;
        //    var user = MembershipService.GetUser(username);
        //    filterContext.HttpContext.User = new Principle(user);
        //}

        protected IMembershipService MembershipService { get; private set; }

        protected User CurrentUser
        {
            get
            {
                if(!User.Identity.IsAuthenticated)
                    throw new InvalidOperationException("User not autorized");
                return User as User;
            }
        }

        protected DataControllerBase(IMembershipService membershipService)
        {
            MembershipService = membershipService;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    UnitOfWork.Dispose();
        //}
    }
}