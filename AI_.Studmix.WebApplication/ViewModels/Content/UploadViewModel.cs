using System;
using System.Collections.Generic;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.ViewModels.Content
{
    public class UploadViewModel
    {
        public IEnumerable<Property> Properties { get; set; }

        /// <summary>
        /// Словарь пары ID свойства - состояние.
        /// </summary>
        public Dictionary<int,string> States { get; set; }

    }
}