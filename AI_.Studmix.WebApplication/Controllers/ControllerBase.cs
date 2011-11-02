using System.Web.Mvc;
using AI_.Studmix.WebApplication.ViewModels.Shared;

namespace AI_.Studmix.WebApplication.Controllers
{
    public abstract class ControllerBase: Controller
    {
        /// <summary>
        /// Сообщение выводимое пользователю.
        /// </summary>
        public void SetMessage(string message)
        {
            TempData["Message"] = message;
        }

        public ViewResult Error(string title,string message)
        {
            var viewModel = new ApplicationErrorViewModel
                                            {
                                                Title = title,
                                                Message = message
                                            };
            return View("ApplicationError", viewModel);
        }
    }
}