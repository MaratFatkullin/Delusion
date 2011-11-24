using System;
using System.Web.Mvc;
using System.Web.Security;
using AI_.Data.Repository;
using AI_.Studmix.ApplicationServices.Services.Abstractions;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.WebApplication.Infrastructure.Authentication;
using AI_.Studmix.WebApplication.ViewModels.Account;

namespace AI_.Studmix.WebApplication.Controllers
{
    public class AccountController : DataControllerBase
    {
        protected IMembershipService MembershipService { get; private set; }
        protected IAuthenticationProvider AuthenticationProvider { get; private set; }
        //
        // GET: /Account/LogOn

        public AccountController(IUnitOfWork unitOfWork,
                                 IMembershipService membershipService,
                                 IAuthenticationProvider authenticationProvider)
            : base(unitOfWork)
        {
            MembershipService = membershipService;
            AuthenticationProvider = authenticationProvider;
        }

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(viewModel.UserName, viewModel.Password))
                {
                    AuthenticationProvider.LogOn(viewModel.UserName, viewModel.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            AuthenticationProvider.LogOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            var viewModel = new RegisterViewModel();
            viewModel.MinRequiredPasswordLength = MembershipService.MinRequiredPasswordLength;
            return View(viewModel);
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;

                var user = MembershipService.CreateUser(model.UserName,
                                                        model.Password,
                                                        model.Email,
                                                        model.PhoneNumber,
                                                        null,
                                                        null,
                                                        true,
                                                        out createStatus);


                if (createStatus == MembershipCreateStatus.Success)
                {
                    AuthenticationProvider.LogOn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    UserPrinciple currentUser = MembershipService.GetUser(User.Identity.Name);
                    changePasswordSucceeded = MembershipService.ChangePassword(currentUser.UserName,
                                                                               viewModel.OldPassword,
                                                                               viewModel.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("",
                                             "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return
                        "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return
                        "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}