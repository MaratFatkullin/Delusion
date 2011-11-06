using System;
using System.Web.Mvc;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
using AI_.Studmix.WebApplication.ViewModels.Finance;
using AI_.Studmix.WebApplication.ViewModels.Shared;

namespace AI_.Studmix.WebApplication.Controllers
{
    public class FinanceController : DataControllerBase
    {
        public FinanceController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public ViewResult Order(int id)
        {
            var package = UnitOfWork.ContentPackageRepository.GetByID(id);
            if (package == null)
                return ErrorView("Материал не найден", "Указанный материал отсутствует в базе данных.");

            var viewModel = new OrderViewModel
                            {
                                OrderPrice = package.Price,
                                UserBalance = CurrentUserProfile.Balance,
                                ContentPackageId = package.ID
                            };

            var financeService = new FinanceService(UnitOfWork);
            var order = new Order
                        {
                            ContentPackage = package,
                            UserProfile = CurrentUserProfile,
                        };
            if(!financeService.IsOrderAvailable(order))
                ModelState.AddModelError("balance","Недостаточно средств для покупки текущего материала.");

            return View(viewModel);
        }

        [HttpPost]
        public ViewResult MakeOrder(OrderViewModel viewModel)
        {
            var packageId = viewModel.ContentPackageId;
            var package = UnitOfWork.ContentPackageRepository.GetByID(packageId);

            var order = new Order
                        {
                            ContentPackage = package,
                            UserProfile = CurrentUserProfile
                        };

            var financeService = new FinanceService(UnitOfWork);
            financeService.MakeOrder(order);


            return InformationView("Покупка успешно произведена.", "",
                                   new ActionLinkInfo("Content",
                                                      "Details",
                                                      "Вернуться к просмотру",
                                                      new {id = package.ID}));
        }
    }
}