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

        public ViewResult ErrorView(string title,string message)
        {
            var viewModel = new ApplicationErrorViewModel
                                            {
                                                Title = title,
                                                Message = message
                                            };
            return View("ApplicationError", viewModel);
        }

        public ViewResult InformationView(string title, string message, ActionLinkInfo link)
        {
            SetMessage(message);
            var viewModel = new InformationViewModel(title);
            viewModel.ActionLinks.Add(link);
            return View("Information", viewModel);
        }
    }
}