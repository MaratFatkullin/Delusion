using System.Security.Principal;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.WebApplication.Infrastructure
{
    public class Principle : IPrincipal
    {
        private readonly IIdentity _identity;
        protected User User { get; private set; }

        public Principle(User user)
        {
            User = user;
            _identity = new Identity(user);
        }

        #region IPrincipal Members

        public bool IsInRole(string role)
        {
            return User.UserPrinciple.IsInRole(role);
        }

        public IIdentity Identity
        {
            get { return _identity; }
        }

        #endregion

        //public static explicit operator User(Principle principle)
        //{
        //    return principle.User;
        //}
    }
}