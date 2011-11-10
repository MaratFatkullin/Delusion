using System.ComponentModel.DataAnnotations;

namespace AI_.Studmix.WebApplication.ViewModels.Account
{
    public class LogOnViewModel
    {
        [Required]
        [Display(Name = "Логин")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}