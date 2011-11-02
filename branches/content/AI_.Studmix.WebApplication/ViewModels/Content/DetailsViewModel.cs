using System.Collections.Generic;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.WebApplication.ViewModels.Content
{
    public class DetailsViewModel
    {
        public ContentPackage Package { get; set; }

        public IEnumerable<Property> Properties { get; set; }
    }
}