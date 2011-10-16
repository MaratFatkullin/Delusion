using System.Collections.Generic;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.ViewModels.Content
{
    public class DownloadViewModel
    {
        public IEnumerable<Property> Properties { get; set; }

        public Dictionary<int,string> States { get; set; }
    }
}