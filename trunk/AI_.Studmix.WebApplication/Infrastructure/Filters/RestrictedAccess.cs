using System.Linq;
using System.Web;
using System.Web.Mvc;
using AI_.Data.Repository;
using AI_.Security.Services;
using AI_.Studmix.Model.DAL.Database;

namespace AI_.Studmix.WebApplication.Infrastructure.Filters
{
    public class RestrictedAccessAttribute : AuthorizeAttribute
    {
        public IUnitOfWork UnitOfWork { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;
            string username = httpContext.User.Identity.Name;
            
            //проверка допустимых имен пользователя
            bool usersOk = true;
            if (!string.IsNullOrEmpty(Users))
            {
                var users = Users.Split(',');
                usersOk = users.Any(user => string.Equals(username, (string) user));
            }
            if (!usersOk)
                return false;

            //проверка допустимых ролей
            bool roleOk = true;
            if (!string.IsNullOrEmpty(Roles))
            {
                using (var unitOfWork = new UnitOfWork<DataContext>())
                {
                    var roleService = new RoleService(unitOfWork);
                    var roles = Roles.Split(',');
                    roleOk = roles.Any(role => roleService.IsUserInRole(username, role.Trim()));
                }
            }
            
            return roleOk;
        }
    }
}