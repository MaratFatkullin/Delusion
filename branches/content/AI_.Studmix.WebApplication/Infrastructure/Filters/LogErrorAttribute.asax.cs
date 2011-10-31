using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace AI_.Studmix.WebApplication.Infrastructure.Filters
{
    public class LogErrorAttribute : HandleErrorAttribute
    {
        private readonly string _policy;

        public LogErrorAttribute(string policy)
        {
            _policy = policy;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = !ExceptionPolicy.HandleException(filterContext.Exception,
                                                                              _policy);
            base.OnException(filterContext);
        }
    }
}