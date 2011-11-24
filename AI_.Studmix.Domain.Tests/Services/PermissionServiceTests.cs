using AI_.Studmix.Domain.Entities;
using AI_.Studmix.Domain.Services;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.Domain.Tests.Services
{
    public class PermissionServiceTestFixture
    {
        protected User User;
        protected ContentPackage Package;
        protected Order Order;

        protected PermissionService PermissionService;

        public PermissionServiceTestFixture()
        {
            User = new User();
            Package = new ContentPackage();
            Order = new Order(User, Package);

            PermissionService = new PermissionService();
        }
    }

    public class PermissionServiceTestTests : PermissionServiceTestFixture
    {
        [Fact]
        public void UserHasPermissions_UserIsOwner_UserHasPermissions()
        {
            // Arrange
            Package.Owner = User;

            // Act
            var userHasPermissions = PermissionService.UserHasPermissions(User, Package);

            // Assert
            userHasPermissions.Should().BeTrue();
        }

        [Fact]
        public void UserHasPermissions_UserOrderedPackage_UserHasPermissions()
        {
            // Arrange
            User.Orders.Add(Order);

            // Act
            var userHasPermissions = PermissionService.UserHasPermissions(User, Package);

            // Assert
            userHasPermissions.Should().BeTrue();
        }

        [Fact]
        public void UserHasPermissions_UserNotOrderedPackage_UserHasPermissions()
        {
            // Arrange

            // Act
            var userHasPermissions = PermissionService.UserHasPermissions(User, Package);

            // Assert
            userHasPermissions.Should().BeFalse();
        }

    }
}