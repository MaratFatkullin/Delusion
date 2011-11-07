using System.Configuration.Provider;
using System.Linq;
using AI_.Security.DAL;
using AI_.Security.Models;

namespace AI_.Security.Services
{
    public class RoleService
    {
        private readonly ISecurityUnitOfWork _unitOfWork;

        public RoleService(ISecurityUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool IsUserInRole(string username, string roleName)
        {
            var role = GetRole(roleName);
            if (role == null)
                throw new ProviderException("Role with specified name does not exists.");

            return role.Users.Any(usr => usr.UserName == username);
        }

        public string[] GetRolesForUser(string username)
        {
            var user = GetUser(username);
            if (user == null)
                throw new ProviderException("User with specified name does not exists.");
            var rolenames = from role in user.Roles
                            select role.RoleName;
            return rolenames.ToArray();
        }

        public void CreateRole(string roleName)
        {
            if (RoleExists(roleName))
                throw new ProviderException("Role with specified name already exists.");

            var role = new Role {RoleName = roleName};
            _unitOfWork.RoleRepository.Insert(role);
            _unitOfWork.Save();
        }

        public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var role = GetRole(roleName);
            if (role == null)
                return false;
            if (role.Users.Count != 0)
            {
                if (throwOnPopulatedRole)
                    throw new ProviderException("Role cannot be deleted cause it has rolemembers.");
                foreach (var user in role.Users)
                    user.Roles.Remove(role);
            }
            _unitOfWork.RoleRepository.Delete(role);
            _unitOfWork.Save();
            return true;
        }

        public bool RoleExists(string roleName)
        {
            var role = _unitOfWork.RoleRepository.Get(r => r.RoleName == roleName).SingleOrDefault();
            return role != null;
        }

        public void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = GetRole(roleName);
                if (role == null)
                    throw new ProviderException("Role with specified name does not exists.");
                foreach (var username in usernames)
                {
                    var userInRole = role.Users.Where(usr => usr.UserName == username).SingleOrDefault();
                    if (userInRole != null)
                        throw new ProviderException("User with specified name already has specified role.");
                    var user = GetUser(username);
                    if (user == null)
                        throw new ProviderException("User with specified name does not exists.");
                    role.Users.Add(user);
                }
            }
            _unitOfWork.Save();
        }

        public void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (var roleName in roleNames)
            {
                var role = GetRole(roleName);
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
            _unitOfWork.Save();
        }

        public string[] GetUsersInRole(string roleName)
        {
            var role = GetRole(roleName);
            if (role == null)
                throw new ProviderException("Role with specified name does not exists.");
            var usernames = from user in role.Users
                            select user.UserName;
            return usernames.ToArray();
        }

        public string[] GetAllRoles()
        {
            return _unitOfWork.RoleRepository
                .Get()
                .Select(role => role.RoleName)
                .ToArray();
        }

        public string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var role = GetRole(roleName);
            if (role == null)
                throw new ProviderException("Role with specified name does not exists.");
            var usernames = from user in role.Users
                            where user.UserName.Contains(usernameToMatch)
                            select user.UserName;
            return usernames.ToArray();
        }

        private User GetUser(string username)
        {
            return _unitOfWork.UserRepository.Get(usr => usr.UserName == username).SingleOrDefault();
        }

        private Role GetRole(string roleName)
        {
            return _unitOfWork.RoleRepository
                .Get(r => r.RoleName == roleName, includeProperties: "Users").FirstOrDefault();
        }
    }
}