using System;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using AI_.Security.DAL;
using AI_.Security.Models;

namespace AI_.Security.Providers
{
    public class CustomRoleProvider : RoleProvider
    {
        private readonly IUnitOfWorkFactory _factory;

        public override string ApplicationName
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public CustomRoleProvider(IUnitOfWorkFactory factory)
        {
            _factory = factory;
        }

        public CustomRoleProvider()
            : this(new UnitOfWorkFactory())
        {
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (var unitOfWork = GetUnitOfWork())
            {
                var role = GetRole(roleName, unitOfWork);
                if (role == null)
                    throw new ProviderException("Role with specified name does not exists.");

                return role.Users.Any(usr => usr.UserName == username);
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var unitOfWork = GetUnitOfWork())
            {
                var user = GetUser(username, unitOfWork);
                if (user == null)
                    throw new ProviderException("User not found.");
                var rolenames = from role in user.Roles
                                select role.RoleName;
                return rolenames.ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            using (var unitOfwork = GetUnitOfWork())
            {
                if (RoleExists(roleName))
                    throw new ProviderException("Role already exists.");

                var role = new Role {RoleName = roleName};
                unitOfwork.RoleRepository.Insert(role);
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            using (var unitOfWork = GetUnitOfWork())
            {
                var role = unitOfWork.RoleRepository.Get(r => r.RoleName == roleName).SingleOrDefault();
                return role != null;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        private ISecurityUnitOfWork GetUnitOfWork()
        {
            return _factory.GetInstance();
        }

        private User GetUser(string username, ISecurityUnitOfWork unitOfWork)
        {
            return unitOfWork.UserRepository.Get(usr => usr.UserName == username).SingleOrDefault();
        }

        private Role GetRole(string roleName, ISecurityUnitOfWork unitOfWork)
        {
            return unitOfWork.RoleRepository
                .Get(r => r.RoleName == roleName, includeProperties: "Users").FirstOrDefault();
        }
    }
}