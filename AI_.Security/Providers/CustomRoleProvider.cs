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
                    throw new ProviderException("User with specified name does not exists.");
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
                    throw new ProviderException("Role with specified name already exists.");

                var role = new Role {RoleName = roleName};
                unitOfwork.RoleRepository.Insert(role);
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (var unitOfWork = GetUnitOfWork())
            {
                var role = GetRole(roleName, unitOfWork);
                if (role == null)
                    return false;
                if (role.Users.Count != 0)
                {
                    if (throwOnPopulatedRole)
                        throw new ProviderException("Role cannot be deleted cause it has rolemembers.");
                    foreach (var user in role.Users)
                        user.Roles.Remove(role);
                }
                unitOfWork.RoleRepository.Delete(role);
                return true;
            }
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
            using (var unitOfWork = GetUnitOfWork())
            {
                foreach (var roleName in roleNames)
                {
                    var role = GetRole(roleName, unitOfWork);
                    if (role == null)
                        throw new ProviderException("Role with specified name does not exists.");
                    foreach (var username in usernames)
                    {
                        var userInRole = role.Users.Where(usr => usr.UserName == username).SingleOrDefault();
                        if (userInRole != null)
                            throw new ProviderException("User with specified name already has specified role.");
                        var user = GetUser(username, unitOfWork);
                            if (user == null)
                                throw new ProviderException("User with specified name does not exists.");
                        role.Users.Add(user);
                    }
                }
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var unitOfWork = GetUnitOfWork())
            {
                foreach (var roleName in roleNames)
                {
                    var role = GetRole(roleName, unitOfWork);
                    if (role == null)
                        throw new ProviderException("Role with specified name does not exists.");
                    foreach (var username in usernames)
                    {
                        var user = role.Users.Where(usr => usr.UserName == username).SingleOrDefault();
                        if (user == null)
                            throw new ProviderException("User with specified name does not have specified role.");
                        role.Users.Remove(user);
                    }
                }
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            using (var unitOfwork = GetUnitOfWork())
            {
                var role = GetRole(roleName, unitOfwork);
                if (role == null)
                    throw new ProviderException("Role with specified name does not exists.");
                var usernames = from user in role.Users
                                select user.UserName;
                return usernames.ToArray();
            }
        }

        public override string[] GetAllRoles()
        {
            using (var unitOfwork = GetUnitOfWork())
                return unitOfwork.RoleRepository
                    .Get()
                    .Select(role => role.RoleName)
                    .ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            using (var unitOfwork = GetUnitOfWork())
            {
                var role = GetRole(roleName, unitOfwork);
                if(role == null)
                    throw new ProviderException("Role with specified name does not exists.");
                var usernames = from user in role.Users
                                where user.UserName.Contains(usernameToMatch)
                                select user.UserName;
                return usernames.ToArray();
            }
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