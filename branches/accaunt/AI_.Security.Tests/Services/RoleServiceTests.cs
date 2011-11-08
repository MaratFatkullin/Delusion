using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using AI_.Data.Repository;
using AI_.Security.Models;
using AI_.Security.Services;
using AI_.Security.Tests.Mocks;
using AI_.Security.Tests.UtilityClasses;
using FluentAssertions;
using Xunit;
using Xunit.Extensions;

namespace AI_.Security.Tests.Services
{
    public class RoleServiceTests
    {
        private readonly RoleService _service;
        private readonly IUnitOfWork _unitOfWork;

        protected ICollection<User> UserStorage
        {
            get { return ((RepositoryMock<User>) _unitOfWork.GetRepository<User>()).Storage; }
        }

        protected ICollection<Role> RoleStorage
        {
            get { return ((RepositoryMock<Role>) _unitOfWork.GetRepository<Role>()).Storage; }
        }

        public RoleServiceTests()
        {
            _unitOfWork = new UnitOfWorkMock();
            _service = new RoleService(_unitOfWork);
        }

        #region Utility Methods

        private void AddUser(User user)
        {
            _unitOfWork.GetRepository<User>().Insert(user);
            _unitOfWork.Save();
        }

        private void AddRole(Role role)
        {
            _unitOfWork.GetRepository<Role>().Insert(role);
            _unitOfWork.Save();
        }

        private void BindUserToRole(User user, Role role)
        {
            user.Roles.Add(role);
            role.Users.Add(user);
        }

        #endregion

        [Fact]
        public void IsUserInRole_UserInRole_TrueReturned()
        {
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            BindUserToRole(user, role);
            AddUser(user);
            AddRole(role);
            var isUserInRole = _service.IsUserInRole(user.UserName, role.RoleName);

            isUserInRole.Should().BeTrue();
        }

        [Fact]
        public void IsUserInRole_UserNotInRole_FalseReturned()
        {
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            AddUser(user);
            AddRole(role);
            var isUserInRole = _service.IsUserInRole(user.UserName, role.RoleName);

            isUserInRole.Should().BeFalse();
        }

        [Fact]
        public void IsUserInRole_RoleNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUser(user);
            _service.Invoking(p => p.IsUserInRole(user.UserName, "roleName"))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void GetRolesForUser_UserHasRoles_RoleNamesReturned()
        {
            var user = UtilityMethods.CreateUser();
            var role1 = UtilityMethods.CreateRole("role1");
            var role2 = UtilityMethods.CreateRole("role2");

            BindUserToRole(user, role1);
            BindUserToRole(user, role2);
            AddUser(user);
            //AddRole(role1);
            //AddRole(role2);
            var rolesForUser = _service.GetRolesForUser(user.UserName);

            rolesForUser.Should().Equal(new List<string> {"role1", "role2"});
        }

        [Fact]
        public void GetRolesForUser_UserHasNoRoles_EmptyCollectionReturned()
        {
            var user = UtilityMethods.CreateUser();
            AddUser(user);
            var rolesForUser = _service.GetRolesForUser(user.UserName);

            rolesForUser.Should().BeEmpty();
        }

        [Fact]
        public void GetRolesForUser_UserNotExists_ExceptionThrown()
        {
            _service.Invoking(p => p.GetRolesForUser("unexistingUsername"))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void CreateRole_Simple_RoleCreated()
        {
            var role = UtilityMethods.CreateRole();
            _service.CreateRole(role.RoleName);

            RoleStorage.Single()
                .ShouldHave()
                .Properties(r => r.RoleName)
                .EqualTo(role);
        }

        [Fact]
        public void CreateRole_RoleWithSameNameExists_ExceptionThrown()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            _service.Invoking(p => p.CreateRole(role.RoleName))
                .ShouldThrow<ProviderException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleExists_RoleDeleted(bool throwOnPopulatedRole)
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            _service.DeleteRole(role.RoleName, throwOnPopulatedRole);

            RoleStorage.Should().BeEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleExists_TrueReturned(bool throwOnPopulatedRole)
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var deleteRole = _service.DeleteRole(role.RoleName, throwOnPopulatedRole);

            deleteRole.Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleNotExists_FalseReturned(bool throwOnPopulatedRole)
        {
            var role = UtilityMethods.CreateRole();
            var roleDeleted = _service.DeleteRole(role.RoleName, throwOnPopulatedRole);

            roleDeleted.Should().BeFalse();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleTrue_ExceptionThrown()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var user = UtilityMethods.CreateUser();
            BindUserToRole(user, role);
            //AddUser(user);

            _service.Invoking(p => p.DeleteRole(role.RoleName, true))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleFalse_RoleDeleted()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var user = UtilityMethods.CreateUser();
            BindUserToRole(user, role);
            //AddUser(user);
            _service.DeleteRole(role.RoleName, false);

            RoleStorage.Should().BeEmpty();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleFalse_UsersExcludedFromRole()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var user = UtilityMethods.CreateUser();
            BindUserToRole(user, role);
            AddUser(user);
            _service.DeleteRole(role.RoleName, false);

            UserStorage.Single().Roles.Should().BeEmpty();
        }

        [Fact]
        public void RoleExists_RoleExists_TrueReturned()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var roleExists = _service.RoleExists(role.RoleName);

            roleExists.Should().BeTrue();
        }

        [Fact]
        public void RoleExists_RoleNotExists_FalseReturned()
        {
            var role = UtilityMethods.CreateRole();
            var roleExists = _service.RoleExists(role.RoleName);

            roleExists.Should().BeFalse();
        }

        [Fact]
        public void AddUsersToRoles_UserDoesNotHaveRole_UsersAddedToRoles()
        {
            var user1 = UtilityMethods.CreateUser("user1");
            var user2 = UtilityMethods.CreateUser("user2");
            var role1 = UtilityMethods.CreateRole("role1");
            var role2 = UtilityMethods.CreateRole("role2");

            AddUser(user1);
            AddUser(user2);
            AddRole(role1);
            AddRole(role2);

            var usernames = new[] {user1.UserName, user2.UserName};
            var roleNames = new[] {role1.RoleName, role2.RoleName};

            _service.AddUsersToRoles(usernames, roleNames);

            RoleStorage.Should().Contain(r => r.Users.Count == 2);
        }

        [Fact]
        public void AddUsersToRoles_UserNotExists_ExceptionThrown()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            _service.Invoking(p => p.AddUsersToRoles(new[] {"unexistingUserName"},
                                                     new[] {role.RoleName}))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void AddUsersToRoles_RoleNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUser(user);

            _service.Invoking(p => p.AddUsersToRoles(new[] {user.UserName},
                                                     new[] {"unexistingRoleName"}))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void AddUsersToRoles_UserHasRole_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            BindUserToRole(user, role);
            AddUser(user);
            AddRole(role);

            _service.Invoking(p => p.AddUsersToRoles(new[] {user.UserName},
                                                     new[] {role.RoleName}))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void RemoveUsersFromRoles_UserHasRole_UsersRemovedFromRoles()
        {
            var user1 = UtilityMethods.CreateUser("user1");
            var user2 = UtilityMethods.CreateUser("user2");
            var role1 = UtilityMethods.CreateRole("role1");
            var role2 = UtilityMethods.CreateRole("role2");

            AddUser(user1);
            AddUser(user2);
            AddRole(role1);
            AddRole(role2);

            BindUserToRole(user1, role1);
            BindUserToRole(user2, role1);
            BindUserToRole(user1, role2);
            BindUserToRole(user2, role2);

            var usernames = new[] {user1.UserName, user2.UserName};
            var roleNames = new[] {role1.RoleName, role2.RoleName};

            _service.RemoveUsersFromRoles(usernames, roleNames);

            RoleStorage.Should().Contain(r => r.Users.Count == 0);
        }

        [Fact]
        public void RemoveUsersFromRoles_UserNotExists_ExceptionThrown()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            _service.Invoking(p => p.RemoveUsersFromRoles(new[] {"unexistingUserName"},
                                                          new[] {role.RoleName}))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void RemoveUsersFromRoles_RoleNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            AddUser(user);

            _service.Invoking(p => p.RemoveUsersFromRoles(new[] {user.UserName},
                                                          new[] {"unexistingRoleName"}))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void RemoveUsersFromRoles_UserDoesNotHaveRole_ExceptionThrown()
        {
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            AddUser(user);
            AddRole(role);

            _service.Invoking(p => p.RemoveUsersFromRoles(new[] {user.UserName},
                                                          new[] {role.RoleName}))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void GetUsersInRole_Simple_UserNamesReturned()
        {
            var role = UtilityMethods.CreateRole();
            var user1 = UtilityMethods.CreateUser("user1");
            var user2 = UtilityMethods.CreateUser("user2");
            BindUserToRole(user1, role);
            BindUserToRole(user2, role);
            AddRole(role);
            var usersInRole = _service.GetUsersInRole(role.RoleName);

            usersInRole.Should().BeEquivalentTo(new List<string> {user1.UserName, user2.UserName});
        }

        [Fact]
        public void GetUsersInRole_NoUsersInRole_EmptyCollectionReturned()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var usersInRole = _service.GetUsersInRole(role.RoleName);

            usersInRole.Should().BeEmpty();
        }

        [Fact]
        public void GetUsersInRole_RoleNotExists_ExceptionThrown()
        {
            _service.Invoking(p => p.GetUsersInRole("unexistingRoleName"))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void GetAllRoles_Simple_RollesReturned()
        {
            AddRole(UtilityMethods.CreateRole("role1"));
            AddRole(UtilityMethods.CreateRole("role2"));
            var allRoles = _service.GetAllRoles();

            allRoles.Should().BeEquivalentTo(new List<string> {"role1", "role2"});
        }

        [Fact]
        public void GetAllRoles_NoRolesExist_EmptyCollectionReturned()
        {
            var allRoles = _service.GetAllRoles();

            allRoles.Should().BeEmpty();
        }

        [Fact]
        public void FindUsersInRole_UsersInRole_MatchedUserNamesReturned()
        {
            var role = UtilityMethods.CreateRole();
            BindUserToRole(UtilityMethods.CreateUser("user1"), role);
            BindUserToRole(UtilityMethods.CreateUser("user2"), role);
            BindUserToRole(UtilityMethods.CreateUser("someOtherMan"), role);
            AddRole(role);
            var usersInRole = _service.FindUsersInRole(role.RoleName, "user");

            usersInRole.Should().BeEquivalentTo(new List<string> {"user1", "user2"});
        }

        [Fact]
        public void FindUsersInRole_NoUsersInRole_EmptyCollectionReturned()
        {
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var usersInRole = _service.FindUsersInRole(role.RoleName, "user");

            usersInRole.Should().BeEmpty();
        }

        [Fact]
        public void FindUsersInRole_RoleNotExists_ExceptionThrown()
        {
            _service.Invoking(p => p.FindUsersInRole("unexistingRoleName", "user"))
                .ShouldThrow<ProviderException>();
        }
    }
}