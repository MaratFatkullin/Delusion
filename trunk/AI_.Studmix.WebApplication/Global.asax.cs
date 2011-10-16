using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AI_.Studmix.WebApplication.DAL.Database;
using AI_.Studmix.WebApplication.DAL.FileSystem;
using AI_.Studmix.WebApplication.Filters;
using AI_.Studmix.WebApplication.Dependencies;
using AI_.Studmix.WebApplication.Infrastructure;
using Microsoft.Practices.Unity;

namespace AI_.Studmix.WebApplication
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new LogErrorAttribute("ErrorFilterPolicy"));
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                // Route name
                "{controller}/{action}/{id}",
                // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
                );
        }

        private static void InitializeDatabase()
        {
            Database.SetInitializer(new CustomDatabaseInitializer());
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();


            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterDependencyResolver();
            RegisterModelBinders();
            InitializeDatabase();
        }

        private void RegisterModelBinders()
        {
            ModelBinders.Binders.Add(typeof (Dictionary<int, string>), new DefaultDictionaryBinder());
        }

        private void RegisterDependencyResolver()
        {
            var container = new UnityContainer();
            ConfigureContainer(container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private void ConfigureContainer(IUnityContainer container)
        {
            container.RegisterType<IControllerFactory, DefaultControllerFactory>();
            container.RegisterType<IControllerActivator, ControllerActivator>();
            container.RegisterType<IViewPageActivator, ViewPageActivator>();
            container.RegisterType<ModelMetadataProvider, DataAnnotationsModelMetadataProvider>();
            container.RegisterType<IFileStorageManager, FileStorageManager>();
            container.RegisterType<IFileStorageProvider, FileStorageProvider>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}