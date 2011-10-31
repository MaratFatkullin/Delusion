using System.Collections.Generic;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.WebApplication.ViewModels.Content
{
    public class SearchViewModel
    {
        public IEnumerable<Property> Properties { get; set; }

        public Dictionary<int,string> States { get; set; }

        public SearchViewModel()
        {
            States = new Dictionary<int, string>();
        }
    }
}