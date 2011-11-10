using System;
using System.Linq;
using AI_.Data.Repository;
using AI_.Security.Models;

namespace AI_.Security.Services
{
    public class RoleService : ServiceBase
    {
        public RoleService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public bool IsUserInRole(string username, string roleName)
        {
            var role = GetRole(roleName);
            if (role == null)
                throw new ArgumentException("Role with specified name does not exists.");

            return role.Users.Any(usr => usr.UserName == username);
        }

        public string[] GetRolesForUser(string username)
        {
            var user = GetUser(username);
            if (user == null)
                throw new ArgumentException("User with specified name does not exists.");
            var rolenames = from role in user.Roles
                            select role.RoleName;
            return rolenames.ToArray();
        }

        public void CreateRole(string roleName)
        {
            if (RoleExists(roleName))
                throw new InvalidOperationException("Role with specified name already exists.");

            var role = new Role {RoleName = roleName};
            UnitOfWork.GetRepository<Role>().Insert(role);
            UnitOfWork.Save();
        }

        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var role = GetRole(roleName);
            if (role == null)
                return false;
            if (role.Users.Count != 0)
            {
                if (throwOnPopulatedRole)
                    throw new InvalidOperationException("Role cannot be deleted cause it has rolemembers.");
                foreach (var user in role.Users)
                    user.Roles.Remove(role);
            }
            UnitOfWork.GetRepository<Role>().Delete(role);
            UnitOfWork.Save();
            return true;
        }

        public bool RoleExists(string roleName)
        {
            var role = UnitOfWork.GetRepository<Role>().Get(r => r.RoleName == roleName).SingleOrDefault();
            return role != null;
        }

        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = GetRole(roleName);
                if (role == null)
                    throw new ArgumentException("Role with specified name does not exists.");
                foreach (var username in usernames)
                {
                    var userInRole = role.Users.Where(usr => usr.UserName == username).SingleOrDefault();
                    if (userInRole != null)
                        throw new InvalidOperationException("User with specified name already has specified role.");
                    var user = GetUser(username);
                    if (user == null)
                        throw new ArgumentException("User with specified name does not exists.");
                    role.Users.Add(user);
                }
            }
            UnitOfWork.Save();
        }

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = GetRole(roleName);
                if (role == null)
                    throw new ArgumentException("Role with specified name does not exists.");
                foreach (var username in usernames)
                {
                    var user = role.Users.Where(usr => usr.UserName == username).SingleOrDefault();
                    if (user == null)
                        throw new ArgumentException("User with specified name does not have specified role.");
                    role.Users.Remove(user);
                }
            }
            UnitOfWork.Save();
        }

        public string[] GetUsersInRole(string roleName)
        {
            var role = GetRole(roleName);
            if (role == null)
                throw new ArgumentException("Role with specified name does not exists.");
            var usernames = from user in role.Users
                            select user.UserName;
            return usernames.ToArray();
        }

        public string[] GetAllRoles()
        {
            return UnitOfWork.GetRepository<Role>()
                .Get()
                .Select(role => role.RoleName)
                .ToArray();
        }

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var role = GetRole(roleName);
            if (role == null)
                throw new ArgumentException("Role with specified name does not exists.");
            var usernames = from user in role.Users
                            where user.UserName.Contains(usernameToMatch)
                            select user.UserName;
            return usernames.ToArray();
        }

        private User GetUser(string username)
        {
            return UnitOfWork.GetRepository<User>().Get(usr => usr.UserName == username).SingleOrDefault();
        }

        private Role GetRole(string roleName)
        {
            return UnitOfWork.GetRepository<Role>()
                .Get(r => r.RoleName == roleName, includeProperties: "Users").FirstOrDefault();
        }
    }
}