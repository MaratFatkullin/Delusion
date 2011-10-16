using System.Collections.Generic;

namespace AI_.Studmix.WebApplication.ViewModels.Content
{
    public class AjaxStatesViewModel
    {
        public IList<PropertyViewModel> Properties { get; set; }

        public AjaxStatesViewModel()
        {
            Properties = new List<PropertyViewModel>();
        }
    }

    public class PropertyViewModel
    {
        public int ID { get; set; }
        public string States { get; set; }
    }
}