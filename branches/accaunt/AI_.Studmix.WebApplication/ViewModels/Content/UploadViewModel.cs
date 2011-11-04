using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.WebApplication.ViewModels.Content
{
    public class UploadViewModel : IValidatableObject
    {
        public IEnumerable<Property> Properties { get; set; }

        /// <summary>
        ///   Словарь пары ID свойства - состояние.
        /// </summary>
        public Dictionary<int, string> States { get; set; }

        [Display(Name = "Контент")]
        public IList<HttpPostedFileBase> ContentFiles { get; set; }

        [Display(Name = "Превью")]
        public IList<HttpPostedFileBase> PreviewContentFiles { get; set; }

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

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ContentFiles.Where(x => x != null).Count() == 0)
                yield return new ValidationResult("Должен быть добавлен хотя бы один файл контента.",
                                                  new[] { "ContentFiles" });
        }

        #endregion
    }
}