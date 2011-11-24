using System.Web.Mvc;
using AI_.Data.Repository;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.Domain.Services;
using AI_.Studmix.Domain.Services.Abstractions;
using AI_.Studmix.WebApplication.ViewModels.Finance;
using AI_.Studmix.WebApplication.ViewModels.Shared;

namespace AI_.Studmix.WebApplication.Controllers
{
    public class FinanceController : DataControllerBase
    {
        public IFinanceService FinanceService { get; set; }

        public FinanceController(IUnitOfWork unitOfWork, IFinanceService financeService)
            : base(unitOfWork)
        {
            FinanceService = financeService;
        }

        [HttpGet]
        public ViewResult Order(int id)
        {
            var package = UnitOfWork.GetRepository<ContentPackage>().GetByID(id);
            if (package == null)
                return ErrorView("Материал не найден", "Указанный материал отсутствует в базе данных.");

            var viewModel = new OrderViewModel
                            {
                                OrderPrice = package.Price,
                                UserBalance = CurrentUser.Balance,
                                ContentPackageId = package.ID
                            };

            var financeService = new FinanceService();
            var order = new Order(CurrentUser, package);
            if (!financeService.IsOrderAvailable(order))
                ModelState.AddModelError("balance", "Недостаточно средств для покупки текущего материала.");

            return View(viewModel);
        }

        [HttpPost]
        public ViewResult MakeOrder(OrderViewModel viewModel)
        {
            var packageId = viewModel.ContentPackageId;
            var package = UnitOfWork.GetRepository<ContentPackage>().GetByID(packageId);

            var order = new Order(CurrentUser, package);

            FinanceService.MakeOrder(order);


            return InformationView("Покупка успешно произведена.",
                                   "",
                                   new ActionLinkInfo("Content",
                                                      "Details",
                                                      "Вернуться к просмотру",
                                                      new {id = package.ID}));
        }
    }
}