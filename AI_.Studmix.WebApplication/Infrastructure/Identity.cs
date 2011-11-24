using System.Security.Principal;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.WebApplication.Infrastructure
{
    internal class Identity : IIdentity
    {
        protected User User { get; set; }

        public Identity(User user)
        {
            User = user;
        }

        #region IIdentity Members

        public string Name
        {
            get { return User.UserPrinciple.UserName; }
        }

        public string AuthenticationType
        {
            get { return "FormAuthentication"; }
        }

        public bool IsAuthenticated
        {
            get { return User != null; }
        }

        #endregion
    }
}