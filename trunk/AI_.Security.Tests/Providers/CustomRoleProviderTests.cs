﻿using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using AI_.Security.DAL;
using AI_.Security.Models;
using AI_.Security.Providers;
using AI_.Security.Tests.Mocks;
using AI_.Security.Tests.UtilityClasses;
using AutoMapper;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Extensions;

namespace AI_.Security.Tests.Providers
{
    public class CustomRoleProviderTests
    {
        private readonly CustomRoleProvider _provider;
        private readonly SecurityUnitOfWorkMock _unitOfWork;

        protected ICollection<User> UserStorage
        {
            get { return ((RepositoryMock<User>) _unitOfWork.UserRepository).Storage; }
        }

        protected ICollection<Role> RoleStorage
        {
            get { return ((RepositoryMock<Role>) _unitOfWork.RoleRepository).Storage; }
        }

        public CustomRoleProviderTests()
        {
            _provider = new CustomRoleProvider();
            _unitOfWork = new SecurityUnitOfWorkMock();
            _provider.Container.RegisterInstance<ISecurityUnitOfWork>(_unitOfWork);

            Mapper.CreateMap<User, User>();
        }

        private void AddUser(User user)
        {
            UserStorage.Add(Mapper.Map<User, User>(user));
        }

        private void AddRole(Role role)
        {
            RoleStorage.Add(Mapper.Map<Role, Role>(role));
        }

        [Fact]
        public void IsUserInRole_UserInRole_TrueReturned()
        {
            var user = UtilityMethods.GetUser();
            var role = UtilityMethods.GetRole();
            user.Roles.Add(role);
            AddUser(user);
            var isUserInRole = _provider.IsUserInRole(user.UserName, role.RoleName);

            isUserInRole.Should().BeTrue();
        }


        [Fact]
        public void IsUserInRole_UserNotInRole_FalseReturned()
        {
            var user = UtilityMethods.GetUser();
            var role = UtilityMethods.GetRole();
            AddUser(user);
            AddRole(role);
            var isUserInRole = _provider.IsUserInRole(user.UserName, role.RoleName);

            isUserInRole.Should().BeFalse();
        }

        [Fact]
        public void IsUserInRole_RoleNotExists_ExceptionThrown()
        {
            var user = UtilityMethods.GetUser();
            AddUser(user);
            _provider.Invoking(p => p.IsUserInRole(user.UserName, "roleName"))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void GetRolesForUser_UserHasRoles_RoleNamesReturned()
        {
            var user = UtilityMethods.GetUser();
            var role1 = UtilityMethods.GetRole("role1");
            var role2 = UtilityMethods.GetRole("role2");

            user.Roles.Add(role1);
            user.Roles.Add(role2);
            AddUser(user);
            AddRole(role1);
            AddRole(role2);
            var rolesForUser = _provider.GetRolesForUser(user.UserName);

            rolesForUser.Should().BeEquivalentTo("role1", "role2");
        }

        [Fact]
        public void GetRolesForUser_UserHasNoRoles_EmptyCollectionReturned()
        {
            var user = UtilityMethods.GetUser();
            AddUser(user);
            var rolesForUser = _provider.GetRolesForUser(user.UserName);

            rolesForUser.Should().BeEmpty();
        }

        [Fact]
        public void CreateRole_Simple_RoleCreated()
        {
            var role = UtilityMethods.GetRole();
            _provider.CreateRole(role.RoleName);

            RoleStorage.Single()
                .ShouldHave()
                .Properties(r => r.RoleName)
                .EqualTo(role);
        }

        [Fact]
        public void CreateRole_RoleWithSameNameExists_ExceptionThrown()
        {
            var role = UtilityMethods.GetRole();
            AddRole(role);

            _provider.Invoking(p => p.CreateRole(role.RoleName))
                .ShouldThrow<ProviderException>();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleExists_RoleDeleted(bool throwOnPopulatedRole)
        {
            var role = UtilityMethods.GetRole();
            AddRole(role);
            _provider.DeleteRole(role.RoleName, throwOnPopulatedRole);

            RoleStorage.Should().BeEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DeleteRole_RoleNotExists_ExceptionThrown(bool throwOnPopulatedRole)
        {
            var role = UtilityMethods.GetRole();

            _provider.Invoking(p => p.DeleteRole(role.RoleName, throwOnPopulatedRole))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleTrue_ExceptionThrown()
        {
            var role = UtilityMethods.GetRole();
            AddRole(role);
            var user = UtilityMethods.GetUser();
            user.Roles.Add(role);
            AddUser(user);

            _provider.Invoking(p => p.DeleteRole(role.RoleName, true))
                .ShouldThrow<ProviderException>();
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleTrue_RoleNotDeleted()
        {
            var role = UtilityMethods.GetRole();
            AddRole(role);
            var user = UtilityMethods.GetUser();
            user.Roles.Add(role);
            AddUser(user);

            RoleStorage.Should().HaveCount(1);
        }

        [Fact]
        public void DeleteRole_ThrowOnPopulatedRoleFalse_RoleDeleted()
        {
            var role = UtilityMethods.GetRole();
            AddRole(role);
            var user = UtilityMethods.GetUser();
            user.Roles.Add(role);
            AddUser(user);

            RoleStorage.Should().BeEmpty();
        }

        [Fact]
        public void RoleExists_RoleExists_TrueReturned()
        {
            var role = UtilityMethods.GetRole();
            AddRole(role);
            var roleExists = _provider.RoleExists(role.RoleName);

            roleExists.Should().BeTrue();
        }

        [Fact]
        public void RoleExists_RoleNotExists_FalseReturned()
        {
            var role = UtilityMethods.GetRole();
            var roleExists = _provider.RoleExists(role.RoleName);

            roleExists.Should().BeFalse();
        }
    }
}