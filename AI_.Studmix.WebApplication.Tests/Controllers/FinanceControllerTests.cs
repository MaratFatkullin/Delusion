using System.Linq;
using System.Web.Mvc;
using AI_.Security.Models;
using AI_.Studmix.Model.Models;
using AI_.Studmix.WebApplication.Controllers;
using AI_.Studmix.WebApplication.Tests.Mocks;
using AI_.Studmix.WebApplication.ViewModels.Finance;
using Moq;
using Xunit;
using FluentAssertions;

namespace AI_.Studmix.WebApplication.Tests.Controllers
{
    public class FinanceControllerTests
    {
        private readonly UnitOfWorkMock _unitOfWork;
        private readonly FinanceController _controller;
        private readonly User _currentUser;
        private readonly UserProfile _currentUserProfile;
        private User _ownerUser;
        private UserProfile _ownerProfile;


        public FinanceControllerTests()
        {
            _unitOfWork = new UnitOfWorkMock();
            _controller = new FinanceController(_unitOfWork);

            _currentUser = new User(){ID = 1 ,UserName = "currentUser"};
            _currentUserProfile = new UserProfile(){ID = 1};
            _controller.ControllerContext = CreateControllerContext(_currentUser,_currentUserProfile);
        }

        private ContentPackage CreatePackage(int id = 1, int price = 100)
        {
            _ownerUser = new User {ID = 2, UserName = "ownerUser"};
            _ownerProfile = new UserProfile {ID = 2, User = _ownerUser};
            _unitOfWork.UserRepository.Insert(_ownerUser);
            _unitOfWork.UserProfileRepository.Insert(_ownerProfile);

            var package = new ContentPackage
                          {
                              ID = id,
                              Price = price,
                              Owner = _ownerUser
                          };

            return package;
        }

        private ControllerContext CreateControllerContext(User user, UserProfile profile)
        {
            profile.User = user;
            _unitOfWork.UserRepository.Insert(user);
            _unitOfWork.Save();
            _unitOfWork.UserProfileRepository.Insert(profile);

            var contextMock = new Mock<ControllerContext>();
            contextMock.Setup(context => context.HttpContext.User.Identity.Name).Returns(user.UserName);
            contextMock.Setup(context => context.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            return contextMock.Object;
        }


        [Fact]
        public void Order_Simple_OrderInformationProvided()
        {
            // Arrange
            var package = CreatePackage();
            _unitOfWork.ContentPackageRepository.Insert(package);
            _unitOfWork.Save();
            _currentUserProfile.Balance = 100;

            // Act
            var result = _controller.Order(package.ID);

            // Assert
            var viewModel = (OrderViewModel) result.Model;
            viewModel.OrderPrice.Should().Be(package.Price);
            viewModel.UserBalance.Should().Be(100);
            viewModel.ContentPackageId.Should().Be(package.ID);
        }

        [Fact]
        public void Order_UserCantMakeOrder_ModelStateErrorRegistered()
        {
            // Arrange
            var package = CreatePackage();
            _unitOfWork.ContentPackageRepository.Insert(package);
            _unitOfWork.Save();
            _currentUserProfile.Balance = 50;

            // Act
            var result = _controller.Order(package.ID);

            // Assert
            result.ViewData.ModelState.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Order_PackageNotExists_OrderInformationProvided()
        {
            // Act
            var result = _controller.Order(-1);

            // Assert
            result.ViewName.Should().Be("ApplicationError");
        }

        [Fact]
        public void MakeOrder_Simple_OrderCreated()
        {
            // Arrange
            var package = CreatePackage();
            _unitOfWork.ContentPackageRepository.Insert(package);
            _unitOfWork.Save();
            _currentUserProfile.Balance = 150;

            // Act
            _controller.MakeOrder(new OrderViewModel {ContentPackageId = package.ID});

            // Assert
            var order = _unitOfWork.OrderRepository.Get().Single();
            order.UserProfile.Should().Be(_currentUserProfile);
            order.ContentPackage.Should().Be(package);
        }

        [Fact]
        public void MakeOrder_Simple_AmountOutcomeFromUserToPackageOwner()
        {
            // Arrange
            var package = CreatePackage();
            _unitOfWork.ContentPackageRepository.Insert(package);
            _unitOfWork.Save();
            _currentUserProfile.Balance = 150;

            // Act
            _controller.MakeOrder(new OrderViewModel { ContentPackageId = package.ID });

            // Assert
            _ownerProfile.Balance.Should().Be(100);
            _currentUserProfile.Balance.Should().Be(50);
        }

    }
}