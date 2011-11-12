using System.Collections.Generic;
using AI_.Security.Models;

namespace AI_.Studmix.WebApplication.ViewModels.Admin
{
    public class UsersViewModel
    {
        public IEnumerable<User> Users { get; set; }

        public int PageSize { get; set; }
    }
}