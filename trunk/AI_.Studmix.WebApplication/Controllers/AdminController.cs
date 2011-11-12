using System.Web.Mvc;
using AI_.Security.Services.Abstractions;
using AI_.Studmix.Model.Services.Abstractions;
using AI_.Studmix.WebApplication.Infrastructure.Filters;
using AI_.Studmix.WebApplication.ViewModels.Admin;

namespace AI_.Studmix.WebApplication.Controllers
{
    [RestrictedAccess(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private const int PAGE_SIZE = 20;
        protected IMembershipService MembershipService { get; private set; }
        protected IProfileService ProfileService { get; private set; }

        public AdminController(IMembershipService membershipService , IProfileService profileService)
        {
            MembershipService = membershipService;
            ProfileService = profileService;
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Users(int id /*pageIndex*/)
        {
            int totalRecords;
            var users = MembershipService.GetAllUsers(id,PAGE_SIZE, out totalRecords);
            var viewModel = new UsersViewModel {Users = users, PageSize = PAGE_SIZE};
            return View(viewModel);
        }

        [HttpGet]
        public ViewResult UserDetails(int id)
        {
            var user = MembershipService.GetUser(id);
            var userProfile = ProfileService.GetUserProfile(user);
            var viewModel = new UserDetailsViewModel
                            {
                                User = user,
                                UserProfile = userProfile
                            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UserDetails(UserDetailsViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var profile = ProfileService.GetUserProfile(viewModel.User.ID);
            profile.Balance = viewModel.UserProfile.Balance;
            ProfileService.Save();

            return RedirectToAction("Users", new {id = 0});
        }
               
    }
}