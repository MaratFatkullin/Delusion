using System.Collections.Generic;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.WebApplication.ViewModels.Content
{
    public class DetailsViewModel
    {
        public ContentPackage Package { get; set; }

        public IEnumerable<Property> Properties { get; set; }

        public bool IsFullAccessGranted { get; set; }
    }
}