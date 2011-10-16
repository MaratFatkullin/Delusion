using System.Web.Mvc;
using AI_.Studmix.WebApplication.DAL.Database;

namespace AI_.Studmix.WebApplication.Controllers
{
    public abstract class DataControllerBase: ControllerBase
    {
        protected IUnitOfWork UnitOfWork { get; private set; }

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