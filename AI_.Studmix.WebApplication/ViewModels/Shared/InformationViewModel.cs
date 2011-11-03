using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AI_.Studmix.WebApplication.ViewModels.Shared
{
    public class InformationViewModel
    {
        public string Title { get; set; }
        public ICollection<ActionLinkInfo> ActionLinks { get; set; }

        public InformationViewModel(string title)
        {
            Title = title;
            ActionLinks = new Collection<ActionLinkInfo>();
        }
    }


    public class ActionLinkInfo
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string LinkText { get; set; }
        public object RouteData { get; set; }

        public ActionLinkInfo(string controllerName,
                              string actionName,
                              string caption,
                              object routeData = null)
        {
            ControllerName = controllerName;
            ActionName = actionName;
            LinkText = caption;
            RouteData = routeData;
        }
    }
}