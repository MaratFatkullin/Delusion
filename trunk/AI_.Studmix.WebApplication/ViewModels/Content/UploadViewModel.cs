﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
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

        public IList<HttpPostedFileBase> ContentFiles { get; set; }

        public IList<HttpPostedFileBase> PreviewContentFiles { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Caption { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Цена")]
        public int Price { get; set; }

        public UploadViewModel()
        {
            States = new Dictionary<int, string>();
            ContentFiles = new List<HttpPostedFileBase>();
            PreviewContentFiles = new List<HttpPostedFileBase>();
        }
    }
}