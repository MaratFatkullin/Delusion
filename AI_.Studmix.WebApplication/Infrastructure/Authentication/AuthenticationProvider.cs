using System.Web.Security;

namespace AI_.Studmix.WebApplication.Infrastructure.Authentication
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        #region IAuthenticationProvider Members

        public void LogOn(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void LogOut()
        {
            FormsAuthentication.SignOut();
        }

        #endregion
    }
}