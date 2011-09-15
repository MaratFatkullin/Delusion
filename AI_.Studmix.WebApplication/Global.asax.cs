using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using AI_.Studmix.WebApplication.DAL;
using AI_.Studmix.WebApplication.Filters;

namespace AI_.Studmix.WebApplication
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogErrorAttribute("ErrorFilterPolicy"));
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        private static void InitializeDatabase()
        {
            Database.SetInitializer(new CustomDatabaseInitializer());
            using (var dataContext = new DataContext())
            {
                dataContext.Database.Initialize(false);
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            InitializeDatabase();
        }
    }
}