using System;
using System.Collections.Generic;
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
            // Arrange
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            BindUserToRole(user, role);
            AddUser(user);
            AddRole(role);

            // Act
            var isUserInRole = _service.IsUserInRole(user.UserName, role.RoleName);

            // Assert
            isUserInRole.Should().BeTrue();
        }

        [Fact]
        public void IsUserInRole_UserNotInRole_FalseReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            AddUser(user);
            AddRole(role);

            // Act
            var isUserInRole = _service.IsUserInRole(user.UserName, role.RoleName);

            // Assert
            isUserInRole.Should().BeFalse();
        }

        [Fact]
        public void IsUserInRole_RoleNotExists_ExceptionThrown()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUser(user);

            // Act, Assert
            _service.Invoking(p => p.IsUserInRole(user.UserName, "roleName"))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetRolesForUser_UserHasRoles_RoleNamesReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            var role1 = UtilityMethods.CreateRole("role1");
            var role2 = UtilityMethods.CreateRole("role2");

            BindUserToRole(user, role1);
            BindUserToRole(user, role2);
            AddUser(user);

            // Act
            var rolesForUser = _service.GetRolesForUser(user.UserName);

            // Assert
            rolesForUser.Should().Equal(new List<string> {"role1", "role2"});
        }

        [Fact]
        public void GetRolesForUser_UserHasNoRoles_EmptyCollectionReturned()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUser(user);

            // Act
            var rolesForUser = _service.GetRolesForUser(user.UserName);

            // Assert
            rolesForUser.Should().BeEmpty();
        }

        [Fact]
        public void GetRolesForUser_UserNotExists_ExceptionThrown()
        {
            // Act, Assert
            _service.Invoking(p => p.GetRolesForUser("unexistingUsername"))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void CreateRole_Simple_RoleCreated()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();

            // Act
            _service.CreateRole(role.RoleName);

            // Assert
            RoleStorage.Single().RoleName.Should().Be(role.RoleName);
        }

        [Fact]
        public void CreateRole_RoleWithSameNameExists_ExceptionThrown()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act, Assert
            _service.Invoking(p => p.CreateRole(role.RoleName))
                .ShouldThrow<InvalidOperationException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleExists_RoleDeleted(bool throwOnPopulatedRole)
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act
            _service.DeleteRole(role.RoleName, throwOnPopulatedRole);

            // Assert
            RoleStorage.Should().BeEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleExists_TrueReturned(bool throwOnPopulatedRole)
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act
            var deleteRole = _service.DeleteRole(role.RoleName, throwOnPopulatedRole);

            // Assert
            deleteRole.Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleNotExists_FalseReturned(bool throwOnPopulatedRole)
        {
            // Arrange
            var role = UtilityMethods.CreateRole();

            // Act
            var roleDeleted = _service.DeleteRole(role.RoleName, throwOnPopulatedRole);

            // Assert
            roleDeleted.Should().BeFalse();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleTrue_ExceptionThrown()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var user = UtilityMethods.CreateUser();
            BindUserToRole(user, role);

            // Act, Assert
            _service.Invoking(p => p.DeleteRole(role.RoleName, true))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleFalse_RoleDeleted()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var user = UtilityMethods.CreateUser();
            BindUserToRole(user, role);

            // Act
            _service.DeleteRole(role.RoleName, false);

            // Assert
            RoleStorage.Should().BeEmpty();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleFalse_UsersExcludedFromRole()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);
            var user = UtilityMethods.CreateUser();
            BindUserToRole(user, role);
            AddUser(user);

            // Act
            _service.DeleteRole(role.RoleName, false);

            // Assert
            UserStorage.Single().Roles.Should().BeEmpty();
        }

        [Fact]
        public void RoleExists_RoleExists_TrueReturned()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act
            var roleExists = _service.RoleExists(role.RoleName);

            // Assert
            roleExists.Should().BeTrue();
        }

        [Fact]
        public void RoleExists_RoleNotExists_FalseReturned()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();

            // Act
            var roleExists = _service.RoleExists(role.RoleName);

            // Assert
            roleExists.Should().BeFalse();
        }

        [Fact]
        public void AddUsersToRoles_UserDoesNotHaveRole_UsersAddedToRoles()
        {
            // Arrange
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

            // Act
            _service.AddUsersToRoles(usernames, roleNames);

            // Assert
            RoleStorage.Should().Contain(r => r.Users.Count == 2);
        }

        [Fact]
        public void AddUsersToRoles_UserNotExists_ExceptionThrown()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act, Assert
            _service.Invoking(p => p.AddUsersToRoles(new[] {"unexistingUserName"},
                                                     new[] {role.RoleName}))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void AddUsersToRoles_RoleNotExists_ExceptionThrown()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUser(user);

            // Act, Assert
            _service.Invoking(p => p.AddUsersToRoles(new[] {user.UserName},
                                                     new[] {"unexistingRoleName"}))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void AddUsersToRoles_UserHasRole_ExceptionThrown()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            BindUserToRole(user, role);
            AddUser(user);
            AddRole(role);

            // Act, Assert
            _service.Invoking(p => p.AddUsersToRoles(new[] {user.UserName},
                                                     new[] {role.RoleName}))
                .ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void RemoveUsersFromRoles_UserHasRole_UsersRemovedFromRoles()
        {
            // Arrange
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

            // Act
            _service.RemoveUsersFromRoles(usernames, roleNames);

            // Assert
            RoleStorage.Should().Contain(r => r.Users.Count == 0);
        }

        [Fact]
        public void RemoveUsersFromRoles_UserNotExists_ExceptionThrown()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act, Assert
            _service.Invoking(p => p.RemoveUsersFromRoles(new[] {"unexistingUserName"},
                                                          new[] {role.RoleName}))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void RemoveUsersFromRoles_RoleNotExists_ExceptionThrown()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            AddUser(user);

            // Act, Assert
            _service.Invoking(p => p.RemoveUsersFromRoles(new[] {user.UserName},
                                                          new[] {"unexistingRoleName"}))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void RemoveUsersFromRoles_UserDoesNotHaveRole_ExceptionThrown()
        {
            // Arrange
            var user = UtilityMethods.CreateUser();
            var role = UtilityMethods.CreateRole();
            AddUser(user);
            AddRole(role);

            // Act, Assert
            _service.Invoking(p => p.RemoveUsersFromRoles(new[] {user.UserName},
                                                          new[] {role.RoleName}))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetUsersInRole_Simple_UserNamesReturned()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            var user1 = UtilityMethods.CreateUser("user1");
            var user2 = UtilityMethods.CreateUser("user2");
            BindUserToRole(user1, role);
            BindUserToRole(user2, role);
            AddRole(role);

            // Act
            var usersInRole = _service.GetUsersInRole(role.RoleName);

            // Assert
            usersInRole.Should().BeEquivalentTo(new List<string> {user1.UserName, user2.UserName});
        }

        [Fact]
        public void GetUsersInRole_NoUsersInRole_EmptyCollectionReturned()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act
            var usersInRole = _service.GetUsersInRole(role.RoleName);

            // Assert
            usersInRole.Should().BeEmpty();
        }

        [Fact]
        public void GetUsersInRole_RoleNotExists_ExceptionThrown()
        {
            // Act, Assert
            _service.Invoking(p => p.GetUsersInRole("unexistingRoleName"))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetAllRoles_Simple_RollesReturned()
        {
            // Arrange
            AddRole(UtilityMethods.CreateRole("role1"));
            AddRole(UtilityMethods.CreateRole("role2"));

            // Act
            var allRoles = _service.GetAllRoles();

            // Assert
            allRoles.Should().BeEquivalentTo(new List<string> {"role1", "role2"});
        }

        [Fact]
        public void GetAllRoles_NoRolesExist_EmptyCollectionReturned()
        {
            // Act
            var allRoles = _service.GetAllRoles();

            // Assert
            allRoles.Should().BeEmpty();
        }

        [Fact]
        public void FindUsersInRole_UsersInRole_MatchedUserNamesReturned()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            BindUserToRole(UtilityMethods.CreateUser("user1"), role);
            BindUserToRole(UtilityMethods.CreateUser("user2"), role);
            BindUserToRole(UtilityMethods.CreateUser("someOtherMan"), role);
            AddRole(role);

            // Act
            var usersInRole = _service.FindUsersInRole(role.RoleName, "user");

            // Assert
            usersInRole.Should().BeEquivalentTo(new List<string> {"user1", "user2"});
        }

        [Fact]
        public void FindUsersInRole_NoUsersInRole_EmptyCollectionReturned()
        {
            // Arrange
            var role = UtilityMethods.CreateRole();
            AddRole(role);

            // Act
            var usersInRole = _service.FindUsersInRole(role.RoleName, "user");

            // Assert
            usersInRole.Should().BeEmpty();
        }

        [Fact]
        public void FindUsersInRole_RoleNotExists_ExceptionThrown()
        {
            // Act, Assert
            _service.Invoking(p => p.FindUsersInRole("unexistingRoleName", "user"))
                .ShouldThrow<ArgumentException>();
        }
    }
}