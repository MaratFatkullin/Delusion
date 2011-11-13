using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AI_.Security.Models;
using AI_.Security.Services.Abstractions;
using AI_.Security.Tests.Mocks;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
using AI_.Studmix.WebApplication.Controllers;
using AI_.Studmix.WebApplication.Infrastructure.Authentication;
using AI_.Studmix.WebApplication.ViewModels.Account;
using FluentAssertions;
using Moq;
using Xunit;

namespace AI_.Studmix.WebApplication.Tests.Controllers
{
    public class AccauntControllerTests
    {
        private Mock<IMembershipService> MembershipServiceMock(User user,
                                                               MembershipCreateStatus membershipCreateStatus)
        {
            var mock = new Mock<IMembershipService>();
            var membershipServiceMock = mock;
            membershipServiceMock.Setup(
                s => s.CreateUser(It.IsAny<string>(),
                                  It.IsAny<string>(),
                                  It.IsAny<string>(),
                                  It.IsAny<string>(),
                                  It.IsAny<string>(),
                                  It.IsAny<bool>(),
                                  out membershipCreateStatus))
                .Returns(user);
            return membershipServiceMock;
        }

        [Fact]
        public void Register_Simple_UserCreated()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var user = new User();
            var membershipServiceMock = MembershipServiceMock(user, MembershipCreateStatus.Success);
            var roleService = new Mock<IRoleService>().Object;
            var authenticationProvider = new Mock<IAuthenticationProvider>().Object;

            var controller = new AccountController(unitOfWork,
                                                   membershipServiceMock.Object,
                                                   roleService,
                                                   new ProfileService(unitOfWork),
                                                   authenticationProvider);
            var viewModel = new RegisterViewModel
            {
                UserName = "username",
                Email = "e@m.l",
                Password = "password",
                ConfirmPassword = "password",
                PhoneNumber = "000000"
            };

            // Act
            controller.Register(viewModel);

            // Assert
            MembershipCreateStatus status;
            membershipServiceMock.Verify(x => x.CreateUser(viewModel.UserName,
                                                           viewModel.Password,
                                                           viewModel.Email,
                                                           null,
                                                           null,
                                                           true,
                                                           out status),
                                         Times.Once());
        }

        [Fact]
        public void Register_UserCreationSucceded_UserProfileCreated()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var user = new User();
            var roleService = new Mock<IRoleService>().Object;
            var membershipServiceMock = MembershipServiceMock(user, MembershipCreateStatus.Success);
            var authenticationProvider = new Mock<IAuthenticationProvider>().Object;

            var controller = new AccountController(unitOfWork,
                                                   membershipServiceMock.Object,
                                                   roleService,
                                                   new ProfileService(unitOfWork),
                                                   authenticationProvider);
            var viewModel = new RegisterViewModel
                            {
                                PhoneNumber = "000000"
                            };

            // Act
            controller.Register(viewModel);

            // Assert
            var userProfile = unitOfWork.GetRepository<UserProfile>().Get().Single();
            userProfile.User.Should().Be(user);
            userProfile.PhoneNumber.Should().Be(viewModel.PhoneNumber);
        }


        [Fact]
        public void Register_UserCreationSucceded_UserRoleAssignedToUser()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var user = new User();
            var membershipServiceMock = MembershipServiceMock(user, MembershipCreateStatus.Success);
            var roleService = new Mock<IRoleService>();
            var authenticationProvider = new Mock<IAuthenticationProvider>().Object;

            var controller = new AccountController(unitOfWork,
                                                   membershipServiceMock.Object,
                                                   roleService.Object,
                                                   new ProfileService(unitOfWork),
                                                   authenticationProvider);

            // Act
            controller.Register(new RegisterViewModel());

            // Assert
            roleService.Verify(rs => rs.AddUsersToRoles(new[] { user.UserName }, new[] { "user" }));
        }

        [Fact]
        public void Register_UserCreationFailed_UserProfileNotCreated()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var user = new User();
            var membershipServiceMock = MembershipServiceMock(user, MembershipCreateStatus.ProviderError);
            var roleService = new Mock<IRoleService>().Object;
            var authenticationProvider = new Mock<IAuthenticationProvider>().Object;

            var controller = new AccountController(unitOfWork,
                                                   membershipServiceMock.Object,
                                                   roleService,
                                                   new ProfileService(unitOfWork),
                                                   authenticationProvider);

            // Act
            controller.Register(new RegisterViewModel());

            // Assert
            var userProfiles = unitOfWork.GetRepository<UserProfile>().Get();
            userProfiles.Should().BeEmpty();
            controller.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Register_ModelStateNotValid_UserNotCreated()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var membershipService = new Mock<IMembershipService>(MockBehavior.Strict).Object;
            var roleService = new Mock<IRoleService>().Object;
            var authenticationProvider = new Mock<IAuthenticationProvider>(MockBehavior.Strict).Object;

            var controller = new AccountController(unitOfWork,
                                                   membershipService,
                                                   roleService,
                                                   new ProfileService(unitOfWork),
                                                   authenticationProvider);
            controller.ModelState.AddModelError("", "");

            // Act
            var result = (ViewResult) controller.Register(new RegisterViewModel());

            // Assert
            result.ViewName.Should().Be("");
        }
    }
}