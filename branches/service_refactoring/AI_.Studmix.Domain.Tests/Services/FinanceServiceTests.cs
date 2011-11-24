using AI_.Studmix.Domain.Entities;
using AI_.Studmix.Domain.Services;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.Domain.Tests.Services
{
    public class FinanceServiceTestFixture
    {
        protected User User;
        protected ContentPackage Package;
        protected Order Order;

        protected FinanceService FinanceService;

        public FinanceServiceTestFixture()
        {
            User = new User();
            Package = new ContentPackage();
            Order = new Order(User, Package);

            FinanceService = new FinanceService();
        }
    }

    public class FinanceServiceTestTests : FinanceServiceTestFixture
    {
   
        [Fact]
        public void MakeOrder_Simple_UserHasOrder()
        {
            // Arrange
            var owner = new User();
            Package.Owner = owner;

            Package.Price = 40;
            User.Balance = 50;
            owner.Balance = 0;

            // Act
            FinanceService.MakeOrder(Order);

            // Assert
            User.Orders.Should().Contain(Order);
            owner.Balance.Should().Be(40);
            User.Balance.Should().Be(10);
        }
    }
}