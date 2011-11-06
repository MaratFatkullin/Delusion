using AI_.Security.Models;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
using AI_.Studmix.WebApplication.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.WebApplication.Tests.Model.Services
{
    public class FinanceServiceTests
    {
        [Fact]
        public void UserHasPermissions_UserIsOwner_UserHasPermissions()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var user = new User();
            var package = new ContentPackage {Owner = user};

            // Act
            var financeService = new FinanceService();
            var userHasPermissions = financeService.UserHasPermissions(unitOfWork, user, package);

            // Assert
            userHasPermissions.Should().BeTrue();
        }

        [Fact]
        public void UserHasPermissions_UserOrderedPackage_UserHasPermissions()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var user = new User();
            var userProfile = new UserProfile {User = user};
            var package = new ContentPackage {Owner = new User()};
            unitOfWork.UserRepository.Insert(user);
            unitOfWork.UserProfileRepository.Insert(userProfile);
            unitOfWork.ContentPackageRepository.Insert(package);

            var order = new Order {ContentPackage = package, UserProfile = userProfile};
            unitOfWork.OrderRepository.Insert(order);
            unitOfWork.Save();

            // Act
            var financeService = new FinanceService();
            var userHasPermissions = financeService.UserHasPermissions(unitOfWork, user, package);

            // Assert
            userHasPermissions.Should().BeTrue();
        }

        [Fact]
        public void UserHasPermissions_UserNotOrderedPackage_UserHasNoPermissions()
        {
            // Arrange
            var unitOfWork = new UnitOfWorkMock();
            var user = new User();
            var userProfile = new UserProfile {User = user};
            var package = new ContentPackage {Owner = new User()};
            unitOfWork.UserRepository.Insert(user);
            unitOfWork.UserProfileRepository.Insert(userProfile);
            unitOfWork.ContentPackageRepository.Insert(package);
            unitOfWork.Save();

            // Act
            var financeService = new FinanceService();
            var userHasPermissions = financeService.UserHasPermissions(unitOfWork, user, package);

            // Assert
            userHasPermissions.Should().BeFalse();
        }
    }
}