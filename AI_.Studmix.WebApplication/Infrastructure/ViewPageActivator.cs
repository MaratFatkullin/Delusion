using System;
using System.Web.Mvc;

namespace AI_.Studmix.WebApplication.Dependencies
{
    public class ViewPageActivator : IViewPageActivator
    {
        public object Create(ControllerContext controllerContext, Type type)
        {
            return DependencyResolver.Current.GetService(type);
        }
    }
}