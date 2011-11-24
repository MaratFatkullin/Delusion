using System.Web.Mvc;
using AI_.Data.Repository;
using AI_.Studmix.ApplicationServices.Services.Abstractions;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.WebApplication.ViewModels.Admin;

namespace AI_.Studmix.WebApplication.Controllers
{
    //[RestrictedAccess(Roles = "admin")]
    public class AdminController : DataControllerBase
    {
        private const int PAGE_SIZE = 20;
        protected IMembershipService MembershipService { get; private set; }

        public AdminController(IUnitOfWork unitOfWork, IMembershipService membershipService)
            : base(unitOfWork)
        {
            MembershipService = membershipService;
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
            var users = MembershipService.GetAllUsers(id, PAGE_SIZE, out totalRecords);
            var viewModel = new UsersViewModel {Users = users, PageSize = PAGE_SIZE};
            return View(viewModel);
        }

        [HttpGet]
        public ViewResult UserDetails(int id)
        {
            var user = UnitOfWork.GetRepository<User>().GetByID(id);
            var viewModel = new UserDetailsViewModel
                            {
                                User = user,
                            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UserDetails(UserDetailsViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            var user = UnitOfWork.GetRepository<User>().GetByID(viewModel.User.ID);
            user.Balance = viewModel.User.Balance;
            UnitOfWork.Save();

            return RedirectToAction("Users", new {id = 0});
        }
    }
}