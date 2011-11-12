using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AI_.Security.Models;
using AI_.Security.Services.Abstractions;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services.Abstractions;
using AI_.Studmix.WebApplication.Controllers;
using AI_.Studmix.WebApplication.ViewModels.Admin;
using Moq;
using Xunit;
using FluentAssertions;

namespace AI_.Studmix.WebApplication.Tests.Controllers
{
    public class AdminControllerTests
    {
        [Fact]
        public void Users_Simple_UserListProvided()
        {
            // Arrange
            var membershipService = new Mock<IMembershipService>();
            int totalRecords = 2;
            membershipService.Setup(us => us.GetAllUsers(It.IsAny<int>(), It.IsAny<int>(), out totalRecords))
                .Returns(new List<User>{new User(),new User()});
            var controller = new AdminController(membershipService.Object,null);

            // Act
            var result = controller.Users(0);

            // Assert
            var viewModel = (UsersViewModel)result.Model;
            viewModel.Users.Should().HaveCount(2);
        }

        [Fact]
        public void Users_Simple_PageSizeInitialized()
        {
            // Arrange
            var membershipService = new Mock<IMembershipService>();
            var controller = new AdminController(membershipService.Object,null);

            // Act
            var result = controller.Users(0);

            // Assert
            var viewModel = (UsersViewModel)result.Model;
            viewModel.PageSize.Should().NotBe(0);
        }

        [Fact]
        public void Users_Simple_UsersInSpecifiedPageProvided()
        {
            // Arrange
            var membershipService = new Mock<IMembershipService>();
            var controller = new AdminController(membershipService.Object,null);

            // Act
            controller.Users(9);

            // Assert
            int totalRecords;
            membershipService.Verify(ms => ms.GetAllUsers(9, It.IsAny<int>(), out totalRecords),Times.Once());
        }

        [Fact]
        public void UserDetails_Simple_UserInformationProvided()
        {
            // Arrange
            var membershipService = new Mock<IMembershipService>();
            var profileService = new Mock<IProfileService>();
            var user = new User();
            var userProfile = new UserProfile();
            membershipService.Setup(s => s.GetUser(3)).Returns(user);
            profileService.Setup(s => s.GetUserProfile(user)).Returns(userProfile);
            var controller = new AdminController(membershipService.Object,profileService.Object);

            // Act
            var result = controller.UserDetails(3);

            // Assert
            var viewModel = (UserDetailsViewModel)result.Model;
            viewModel.User.Should().Be(user);
            viewModel.UserProfile.Should().Be(userProfile);
        }

        [Fact]
        public void UserDetailsPost_ModelStateIsValid_UserProfileUpdated()
        {
            //// Arrange
            //var profile = new UserProfile();
            //var viewModel = new UserDetailsViewModel {UserProfile = profile};
            //var profileService = new Mock<IProfileService>();
            //var controller = new AdminController(null, profileService.Object);

            //// Act
            //controller.UserDetails(viewModel);

            //// Assert
            //profileService.Verify(s=>s.UpdateUserProfile(profile));
            throw new NotImplementedException();
        }

        [Fact]
        public void UserDetailsPost_ModelStateIsValid_RedirectedToUserList()
        {
            // Arrange
            var profile = new UserProfile();
            var viewModel = new UserDetailsViewModel { UserProfile = profile };
            var profileService = new Mock<IProfileService>();
            var controller = new AdminController(null, profileService.Object);

            // Act
            var result = controller.UserDetails(viewModel);

            // Assert
            var viewResult = (ViewResult)result ;
            viewResult.ViewName.Should().Be("Users");
        }

        [Fact]
        public void UserDetailsPost_ModelStateIsNotValid_FormReshown()
        {
            // Arrange
            var viewModel = new UserDetailsViewModel {UserProfile = new UserProfile(), User = new User()};
            var profileService = new Mock<IProfileService>();
            var controller = new AdminController(null, profileService.Object);
            controller.ModelState.AddModelError("","");

            // Act
            var result = controller.UserDetails(viewModel);

            // Assert
            var viewResult = (ViewResult)result;
            viewResult.ViewName.Should().Be("");
        }
    }
}