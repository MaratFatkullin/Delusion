using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AI_.Studmix.WebApplication.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} должен быть хотя бы {2} символов длинной.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Введенные пароли не совпадают.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Телефон")]
        public string PhoneNumber { get; set; }

        public int MinRequiredPasswordLength { get; set; }
    }
}
