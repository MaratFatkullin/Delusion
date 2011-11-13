using AI_.Security.Models;
using AI_.Security.Tests.Mocks;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
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
            var financeService = new FinanceService(unitOfWork);
            var userHasPermissions = financeService.UserHasPermissions(user, package);

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
            unitOfWork.GetRepository<User>().Insert(user);
            unitOfWork.GetRepository<UserProfile>().Insert(userProfile);
            unitOfWork.GetRepository<ContentPackage>().Insert(package);

            var order = new Order {ContentPackage = package, UserProfile = userProfile};
            unitOfWork.GetRepository<Order>().Insert(order);
            unitOfWork.Save();

            // Act
            var financeService = new FinanceService(unitOfWork);
            var userHasPermissions = financeService.UserHasPermissions(user, package);

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
            unitOfWork.GetRepository<User>().Insert(user);
            unitOfWork.GetRepository<UserProfile>().Insert(userProfile);
            unitOfWork.GetRepository<ContentPackage>().Insert(package);
            unitOfWork.Save();

            // Act
            var financeService = new FinanceService(unitOfWork);
            var userHasPermissions = financeService.UserHasPermissions(user, package);

            // Assert
            userHasPermissions.Should().BeFalse();
        }
    }
}