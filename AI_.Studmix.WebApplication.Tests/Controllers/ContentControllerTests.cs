using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AI_.Security.Models;
using AI_.Studmix.Model.Models;
using AI_.Studmix.WebApplication.Controllers;
using AI_.Studmix.WebApplication.Tests.Mocks;
using AI_.Studmix.WebApplication.ViewModels.Content;
using FluentAssertions;
using Moq;
using Xunit;

namespace AI_.Studmix.WebApplication.Tests.Controllers
{
    public class ContentControllerTests
    {
        private readonly ContentController _controller;
        private readonly FileStorageManagerMock _fileStorageManager;
        private readonly UnitOfWorkMock _unitOfWork;


        public ContentControllerTests()
        {
            _unitOfWork = new UnitOfWorkMock();
            _fileStorageManager = new FileStorageManagerMock();
            _controller = new ContentController(_unitOfWork, _fileStorageManager);
            _controller.ControllerContext = CreateControllerContext();
            InitUnitOfWork(_unitOfWork);
        }

        #region Utility methods

        private void InitUnitOfWork(UnitOfWorkMock unitOfWork)
        {
            var property1 = new Property {Name = "property1", Order = 1, ID = 1};
            var property2 = new Property {Name = "property2", Order = 2, ID = 2};

            var state1 = new PropertyState {Property = property1, Value = "state1"};
            var state2 = new PropertyState {Property = property1, Value = "state2"};
            var state3 = new PropertyState {Property = property2, Value = "state3"};
            var state4 = new PropertyState {Property = property2, Value = "state4"};

            property1.States = new List<PropertyState> { state1, state2 };
            property2.States = new List<PropertyState> { state3, state4 };

            var package1 = new ContentPackage
                           {
                               PropertyStates = new Collection<PropertyState> {state1, state3}
                           };

            var package2 = new ContentPackage
                           {
                               PropertyStates = new Collection<PropertyState> {state2, state4}
                           };

            state1.ContentPackages = new Collection<ContentPackage> { package1 };
            state2.ContentPackages = new Collection<ContentPackage> { package2 };
            state3.ContentPackages = new Collection<ContentPackage> { package1 };
            state4.ContentPackages = new Collection<ContentPackage> { package2 };

            unitOfWork.PropertyRepository.Insert(property1);
            unitOfWork.PropertyRepository.Insert(property2);

            unitOfWork.PropertyStateRepository.Insert(state1);
            unitOfWork.PropertyStateRepository.Insert(state2);
            unitOfWork.PropertyStateRepository.Insert(state3);
            unitOfWork.PropertyStateRepository.Insert(state4);

            unitOfWork.ContentPackageRepository.Insert(package1);
            unitOfWork.ContentPackageRepository.Insert(package2);
        }

        private static MemoryStream CreateInputStream(string data = "mockedData")
        {
            return new MemoryStream(Encoding.ASCII.GetBytes(data));
        }

        private HttpPostedFileMock CreateHttpPostedFile(string filename = "file.txt")
        {
            return new HttpPostedFileMock(filename, CreateInputStream());
        }

        private HttpPostedFileMock CreateHttpPostedFile(Stream stream)
        {
            return new HttpPostedFileMock("file.txt", stream);
        }

        private ControllerContext CreateControllerContext(string username = "user")
        {
            var user = new User {UserName = username};
            _unitOfWork.UserRepository.Insert(user);
            _unitOfWork.UserProfileRepository.Insert(new UserProfile {User = user});
            var contextMock = new Mock<ControllerContext>();
            contextMock.Setup(context => context.HttpContext.User.Identity.Name).Returns(username);
            contextMock.Setup(context => context.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            return contextMock.Object;
        }

        #endregion

        [Fact]
        public void UpdateStates_AllPropertiesUnspecified_AllStatesOfFirstPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string>(), 1);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().Be("state1|state2");
        }

        [Fact]
        public void UpdateStates_AllPropertiesUnspecified_AllStatesOfSecondPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string>(), 2);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().Be("state3|state4");
        }

        [Fact]
        public void UpdateStates_FirstPropertySpecified_AllStatesOfFirstPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string> {{1, "state1"}}, 1);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().Be("state1|state2");
        }

        [Fact]
        public void UpdateStates_FirstPropertySpecified_OnlyBoundedStatesOfSecondPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string> {{1, "state1"}}, 2);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().Be("state3");
        }

        [Fact]
        public void UpdateStates_SecondPropertySpecified_AllStatesOfFirstPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string> {{2, "state3"}}, 1);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().Be("state1|state2");
        }

        [Fact]
        public void UpdateStates_SecondPropertySpecified_AllStatesOfSecondPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string> {{2, "state3"}}, 2);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().Be("state3|state4");
        }

        [Fact]
        public void UpdateStates_SecondPropertySettedInNewStateValue_AllStatesOfFirstPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string> {{2, "newState"}}, 1);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().Be("state1|state2");
        }


        [Fact]
        public void UpdateStates_FirstPropertySettedInNewStateValue_NoStatesOfSecondPropertyAvailable()
        {
            // Arrange

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string> {{1, "newState"}}, 2);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().BeEmpty();
        }

        [Fact]
        public void UpdateStates_NotAllPropertiesSpecifiedInPackages_NoStatesForUnspecifiedPropertieAvailable()
        {
            // Arrange
            _unitOfWork.PropertyRepository.Insert(new Property {Name = "property", Order = 3,ID = 3});

            // Act
            var viewResult = _controller.UpdateStates(new Dictionary<int, string> {{1, "state1"}}, 3);

            // Assert
            var states = (string) viewResult.Data;
            states.Should().BeEmpty();
        }

        [Fact]
        public void UploadPost_SinglePostedContentFile_PostedFileStored()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            var httpPostedFile = CreateHttpPostedFile();
            viewModel.ContentFiles = new List<HttpPostedFileBase> {httpPostedFile};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _fileStorageManager.Package.Files.Single().Name.Should().Be(httpPostedFile.FileName);
        }

        [Fact]
        public void UploadPost_SinglePostedContentFile_ContentPackageStoredToDatabase()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.ContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            var result = _unitOfWork.ContentPackageRepository.Get();
            result.Should().HaveCount(3);
        }

        [Fact]
        public void UploadPost_SinglePostedContentFile_PostedFileNotPreview()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.ContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _fileStorageManager.Package.Files.Single().IsPreview.Should().BeFalse();
        }

        [Fact]
        public void UploadPost_TwoPostedFile_BothPostedFileStored()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.ContentFiles = new List<HttpPostedFileBase>
                                     {
                                         CreateHttpPostedFile(),
                                         CreateHttpPostedFile()
                                     };

            // Act
            _controller.Upload(viewModel);

            // Assert
            _fileStorageManager.Package.Files.Should().HaveCount(2);
        }

        [Fact]
        public void UploadPost_SinglePostedPreviewFile_PostedFileIsPreview()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _fileStorageManager.Package.Files.Single().IsPreview.Should().BeTrue();
        }

        [Fact]
        public void UploadPost_ThereIsEmptyPropertyStates_EmptyPropertyStatesSkiped()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};
            viewModel.States = new Dictionary<int, string> {{1, string.Empty}, {2, "state3"}};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _fileStorageManager.Package.PropertyStates.Single().Value.Should().Be("state3");
        }

        [Fact]
        public void UploadPost_NewPropertyStates_NewPropertyStatesStoredToDatabase()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};
            viewModel.States = new Dictionary<int, string> {{1, "newState"}};

            // Act
            _controller.Upload(viewModel);

            // Assert
            var result = _unitOfWork.PropertyStateRepository.Get();
            result.Last().Value.Should().Be("newState");
        }

        [Fact]
        public void UploadPost_NewPropertyStates_NewPropertyStatesHasInitializedIndex()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};
            viewModel.States = new Dictionary<int, string> {{1, "newState"}};

            // Act
            _controller.Upload(viewModel);

            // Assert
            var repositoryMock = _unitOfWork.ContentPackageRepository;
            repositoryMock.Get().Last().PropertyStates.Should().OnlyContain(x => x.Index != 0);
        }

        [Fact]
        public void UploadPost_Simple_ContentPackagesStoredToDatabase()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            var result = _unitOfWork.ContentPackageRepository.Get();
            result.Should().HaveCount(3);
        }

        [Fact]
        public void UploadPost_Simple_ContentFileHasSpecifiedFileName()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase>
                                            {
                                                CreateHttpPostedFile(filename: "filename")
                                            };

            // Act
            _controller.Upload(viewModel);

            // Assert
            _fileStorageManager.Package.Files.Single().Name.Should().Be("filename");
        }


        [Fact]
        public void UploadPost_Simple_ContentFileHasSpecifiedStream()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            var stream = CreateInputStream();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile(stream)};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _fileStorageManager.Package.Files.Single().Stream.Should().Be(stream);
        }

        [Fact]
        public void UploadPost_Simple_OwnerOfPackageIsCurrentUser()
        {
            // Arrange
            _controller.ControllerContext = CreateControllerContext("username");
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _unitOfWork.ContentPackageRepository.Get().Last().Owner.UserName.Should().Be("username");
        }

        [Fact]
        public void UploadPost_Simple_CaptionOfPackageInitialized()
        {
            // Arrange
            var viewModel = new UploadViewModel {Caption = "caption"};
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _unitOfWork.ContentPackageRepository.Get().Last().Caption.Should().Be("caption");
        }


        [Fact]
        public void UploadPost_Simple_DescriptionOfPackageInitialized()
        {
            // Arrange
            var viewModel = new UploadViewModel {Description = "description"};
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _unitOfWork.ContentPackageRepository.Get().Last().Description.Should().Be("description");
        }


        [Fact]
        public void UploadPost_Simple_PriceOfPackageInitialized()
        {
            // Arrange
            var viewModel = new UploadViewModel {Price = 10};
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            _unitOfWork.ContentPackageRepository.Get().Last().Price.Should().Be(10);
        }

        [Fact]
        public void UploadPost_NoFilesPosted_ContentPackagesNotStoredToDatabase()
        {
            // Arrange
            _controller.ModelState.AddModelError("ContentFiles", "errorMessage");
            // Act
            _controller.Upload(new UploadViewModel());

            // Assert
            var repositoryMock = _unitOfWork.ContentPackageRepository;
            repositoryMock.Get().Should().HaveCount(2);
        }

        [Fact]
        public void UploadPost_NoFilesPosted_ContentPackagesNotStoredToFileSystem()
        {
            // Arrange
            _controller.ModelState.AddModelError("ContentFiles", "errorMessage");

            // Act
            _controller.Upload(new UploadViewModel());

            // Assert
            _fileStorageManager.Package.Should().BeNull();
        }


        [Fact]
        public void Search_Simple_PropertiesInitialized()
        {
            // Arrange
            _controller.ModelState.AddModelError("ContentFiles", "errorMessage");

            // Act
            var result = _controller.Search();

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Properties.Should().NotBeEmpty();
        }
    }
}