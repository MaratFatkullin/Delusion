using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AI_.Studmix.WebApplication.Controllers;
using AI_.Studmix.WebApplication.Models;
using AI_.Studmix.WebApplication.Tests.Mocks;
using AI_.Studmix.WebApplication.ViewModels.Content;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.WebApplication.Tests.Controllers
{
    public class ContentControllerTests
    {
        private readonly ContentController _controller;
        private readonly UnitOfWorkMock _unitOfWork;


        public ContentControllerTests()
        {
            _unitOfWork = new UnitOfWorkMock();
            _controller = new ContentController(_unitOfWork);
            InitUnitOfWork(_unitOfWork);
        }

        #region Utility methods

        private void InitUnitOfWork(UnitOfWorkMock unitOfWork)
        {
            var property1 = new Property {Name = "property1", Order = 1};
            var property2 = new Property {Name = "property2", Order = 2};

            var state1 = new PropertyState {Property = property1, Value = "state1"};
            var state2 = new PropertyState {Property = property1, Value = "state2"};
            var state3 = new PropertyState {Property = property2, Value = "state3"};
            var state4 = new PropertyState {Property = property2, Value = "state4"};

            property1.States = new List<PropertyState> {state1, state2};
            property2.States = new List<PropertyState> {state3, state4};

            var package1 = new ContentPackage
                           {
                               PropertyStates = new Collection<PropertyState> {state1, state3}
                           };

            var package2 = new ContentPackage
                           {
                               PropertyStates = new Collection<PropertyState> {state2, state4}
                           };


            unitOfWork.PropertyRepository.Insert(property1);
            unitOfWork.PropertyRepository.Insert(property2);

            unitOfWork.PropertyStateRepository.Insert(state1);
            unitOfWork.PropertyStateRepository.Insert(state2);
            unitOfWork.PropertyStateRepository.Insert(state3);
            unitOfWork.PropertyStateRepository.Insert(state4);

            unitOfWork.ContentPackageRepository.Insert(package1);
            unitOfWork.ContentPackageRepository.Insert(package2);
        }

        #endregion

        [Fact]
        public void UpdateStates_AllPropertiesUnspecified_AllStatesOfFirstPropertyAvailable()
        {
            // Arrange
            var uploadViewModel = new UploadViewModel();
            uploadViewModel.States = new Dictionary<int, string>();

            // Act
            var viewResult = _controller.UpdateStates(uploadViewModel);

            // Assert
            var viewModel = (AjaxStatesViewModel) viewResult.Data;
            viewModel.Properties.First().States.Should().Be("state1|state2");
        }


        [Fact]
        public void UpdateStates_AllPropertiesUnspecified_AllStatesOfSecondPropertyAvailable()
        {
            // Arrange
            var uploadViewModel = new UploadViewModel();
            uploadViewModel.States = new Dictionary<int, string>();

            // Act
            var viewResult = _controller.UpdateStates(uploadViewModel);

            // Assert
            var viewModel = (AjaxStatesViewModel) viewResult.Data;
            viewModel.Properties.Last().States.Should().Be("state3|state4");
        }

        [Fact]
        public void UpdateStates_FirstPropertySpecified_AllStatesOfFirstPropertyAvailable()
        {
            // Arrange
            var uploadViewModel = new UploadViewModel();
            uploadViewModel.States = new Dictionary<int, string> {{1, "state1"}};

            // Act
            var viewResult = _controller.UpdateStates(uploadViewModel);

            // Assert
            var viewModel = (AjaxStatesViewModel) viewResult.Data;
            viewModel.Properties.First().States.Should().Be("state1|state2");
        }

        [Fact]
        public void UpdateStates_FirstPropertySpecified_OnlyBoundedStatesOfSecondPropertyAvailable()
        {
            // Arrange
            var uploadViewModel = new UploadViewModel();
            uploadViewModel.States = new Dictionary<int, string> {{1, "state1"}};

            // Act
            var viewResult = _controller.UpdateStates(uploadViewModel);

            // Assert
            var viewModel = (AjaxStatesViewModel) viewResult.Data;
            viewModel.Properties.Last().States.Should().Be("state3");
        }

        [Fact]
        public void UpdateStates_SecondPropertySpecified_AllStatesOfFirstPropertyAvailable()
        {
            // Arrange
            var uploadViewModel = new UploadViewModel();
            uploadViewModel.States = new Dictionary<int, string> {{2, "state3"}};

            // Act
            var viewResult = _controller.UpdateStates(uploadViewModel);

            // Assert
            var viewModel = (AjaxStatesViewModel) viewResult.Data;
            viewModel.Properties.First().States.Should().Be("state1|state2");
        }

        [Fact]
        public void UpdateStates_SecondPropertySpecified_AllStatesOfSecondPropertyAvailable()
        {
            // Arrange
            var uploadViewModel = new UploadViewModel();
            uploadViewModel.States = new Dictionary<int, string> {{2, "state3"}};

            // Act
            var viewResult = _controller.UpdateStates(uploadViewModel);

            // Assert
            var viewModel = (AjaxStatesViewModel) viewResult.Data;
            viewModel.Properties.Last().States.Should().Be("state3|state4");
        }

        [Fact]
        public void UpdateStates_NotAllPropertiesSpecifiedInPackages_NoStatesForUnspecifiedPropertieAvailable()
        {
            // Arrange
            var uploadViewModel = new UploadViewModel();
            _unitOfWork.PropertyRepository.Insert(new Property {Name = "property", Order = 3});
            uploadViewModel.States = new Dictionary<int, string> {{1, "state1"}};

            // Act
            var viewResult = _controller.UpdateStates(uploadViewModel);

            // Assert
            var viewModel = (AjaxStatesViewModel) viewResult.Data;
            viewModel.Properties.Last().States.Should().BeEmpty();
        }
    }
}