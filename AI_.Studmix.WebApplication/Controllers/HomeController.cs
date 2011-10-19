using System.Web.Mvc;

namespace AI_.Studmix.WebApplication.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to Studmix!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
