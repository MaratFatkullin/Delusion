using System;
using System.Configuration.Provider;
using System.Web.Security;
using AI_.Security.DAL;
using AI_.Security.Models;
using System.Linq;

namespace AI_.Security.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        public CustomRoleProvider(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public CustomRoleProvider()
            :this(new UnitOfWorkFactory())
        {
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var unitOfWork = GetUnitOfWork())
            {
                Role role = GetRole(roleName, unitOfWork);
                if (role == null)
                    throw new ProviderException("Role with specified name does not exists.");

                return role.Users.Any(usr => usr.UserName == username);
            }
        }

        private Role GetRole(string roleName, ISecurityUnitOfWork unitOfWork)
        {
            return unitOfWork.RoleRepository
                .Get(r => r.RoleName == roleName,includeProperties:"Users").FirstOrDefault();
        }

        public override string[] GetRolesForUser(string username)
        {
            throw new System.NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        public override string ApplicationName
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        private IUnitOfWorkFactory _factory;

        private ISecurityUnitOfWork GetUnitOfWork()
        {
            return _factory.GetInstance();
        }
    }
}