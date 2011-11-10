using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AI_.Security.Models;
using AI_.Security.Tests.Mocks;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services;
using AI_.Studmix.Model.Services.Abstractions;
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
        private readonly User _currentUser;
        private readonly UserProfile _currentUserProfile;
        private readonly FileStorageManagerMock _fileStorageManager;
        private readonly UnitOfWorkMock _unitOfWork;
        private ContentController _controller;


        public ContentControllerTests()
        {
            _unitOfWork = new UnitOfWorkMock();
            _fileStorageManager = new FileStorageManagerMock();

            _currentUser = new User {UserName = "currentusername"};
            _currentUserProfile = new UserProfile {User = _currentUser};
            _unitOfWork.GetRepository<User>().Insert(_currentUser);
            _unitOfWork.GetRepository<UserProfile>().Insert(_currentUserProfile);

            _controller = new ContentController(_unitOfWork,
                                                _fileStorageManager,
                                                new FinanceService(_unitOfWork));
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

            property1.States = new List<PropertyState> {state1, state2};
            property2.States = new List<PropertyState> {state3, state4};

            var package1 = new ContentPackage
                           {
                               ID = 1,
                               PropertyStates = new Collection<PropertyState> {state1, state3}
                           };

            var package2 = new ContentPackage
                           {
                               ID = 2,
                               PropertyStates = new Collection<PropertyState> {state2, state4}
                           };

            state1.ContentPackages = new Collection<ContentPackage> {package1};
            state2.ContentPackages = new Collection<ContentPackage> {package2};
            state3.ContentPackages = new Collection<ContentPackage> {package1};
            state4.ContentPackages = new Collection<ContentPackage> {package2};

            unitOfWork.GetRepository<Property>().Insert(property1);
            unitOfWork.GetRepository<Property>().Insert(property2);

            unitOfWork.GetRepository<PropertyState>().Insert(state1);
            unitOfWork.GetRepository<PropertyState>().Insert(state2);
            unitOfWork.GetRepository<PropertyState>().Insert(state3);
            unitOfWork.GetRepository<PropertyState>().Insert(state4);

            unitOfWork.GetRepository<ContentPackage>().Insert(package1);
            unitOfWork.GetRepository<ContentPackage>().Insert(package2);

            unitOfWork.Save();
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

        private ControllerContext CreateControllerContext(string role = "user")
        {
            var contextMock = new Mock<ControllerContext>();
            contextMock.Setup(context => context.HttpContext.User.Identity.Name).Returns(_currentUser.UserName);
            contextMock.Setup(context => context.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            contextMock.Setup(context => context.HttpContext.User.IsInRole(role)).Returns(true);
            return contextMock.Object;
        }

        private static Mock<IFinanceService> GetFinanceServiceMock(bool isPermissionsGranted = true)
        {
            var serviceMock = new Mock<IFinanceService>();
            serviceMock.Setup(m =>
                              m.UserHasPermissions(It.IsAny<User>(),
                                                   It.IsAny<ContentPackage>()))
                .Returns(isPermissionsGranted);
            return serviceMock;
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
            _unitOfWork.GetRepository<Property>().Insert(new Property {Name = "property", Order = 3, ID = 3});
            _unitOfWork.Save();

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
            var result = _unitOfWork.GetRepository<ContentPackage>().Get();
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
            var result = _unitOfWork.GetRepository<PropertyState>().Get();
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
            var repositoryMock = _unitOfWork.GetRepository<ContentPackage>();
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
            var result = _unitOfWork.GetRepository<ContentPackage>().Get();
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
            var viewModel = new UploadViewModel();
            viewModel.PreviewContentFiles = new List<HttpPostedFileBase> {CreateHttpPostedFile()};

            // Act
            _controller.Upload(viewModel);

            // Assert
            var package = _unitOfWork.GetRepository<ContentPackage>().Get().Last();
            package.Owner.Should().Be(_currentUser);
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
            _unitOfWork.GetRepository<ContentPackage>().Get().Last().Caption.Should().Be("caption");
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
            _unitOfWork.GetRepository<ContentPackage>().Get().Last().Description.Should().Be("description");
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
            _unitOfWork.GetRepository<ContentPackage>().Get().Last().Price.Should().Be(10);
        }

        [Fact]
        public void UploadPost_NoFilesPosted_ContentPackagesNotStoredToDatabase()
        {
            // Arrange
            _controller.ModelState.AddModelError("ContentFiles", "errorMessage");
            // Act
            _controller.Upload(new UploadViewModel());

            // Assert
            var repositoryMock = _unitOfWork.GetRepository<ContentPackage>();
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

            // Act
            var result = _controller.Search();

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Properties.Should().HaveCount(2);
        }

        [Fact]
        public void SearchPost_AllPropertySpecified_PackageFound()
        {
            // Arrange
            var viewModel = new SearchViewModel();
            viewModel.States = new Dictionary<int, string> {{1, "state1"}, {2, "state2"}};

            // Act
            var result = _controller.Search(viewModel);

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Packages.Single().ID.Should().Be(1);
        }

        [Fact]
        public void SearchPost_NotAllPropertySpecified_PackageFound()
        {
            // Arrange
            var viewModel = new SearchViewModel();
            viewModel.States = new Dictionary<int, string> {{1, ""}, {2, "state4"}};

            // Act
            var result = _controller.Search(viewModel);

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Packages.Single().ID.Should().Be(2);
        }

        [Fact]
        public void SearchPost_ManyMatchedPackages_AllMatchedPackagesFound()
        {
            // Arrange
            var property = _unitOfWork.GetRepository<Property>().GetByID(1);
            var state1 = property.States.First();
            var state2 = property.States.Last();
            var newPackage = new ContentPackage
                             {ID = 3, PropertyStates = new Collection<PropertyState> {state1, state2}};
            state1.ContentPackages.Add(newPackage);
            state2.ContentPackages.Add(newPackage);
            _unitOfWork.GetRepository<ContentPackage>().Insert(newPackage);

            var viewModel = new SearchViewModel();
            viewModel.States = new Dictionary<int, string> {{1, "state1"}, {2, ""}};

            // Act
            var result = _controller.Search(viewModel);

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Packages.Should().HaveCount(2);
        }

        [Fact]
        public void SearchPost_NoMatchedPackages_NoPackageFound()
        {
            // Arrange
            var viewModel = new SearchViewModel();
            viewModel.States = new Dictionary<int, string> {{1, "state1"}, {2, "state4"}};

            // Act
            var result = _controller.Search(viewModel);

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Packages.Should().BeEmpty();
        }

        [Fact]
        public void SearchPost_Simple_PropertiesInitialized()
        {
            // Arrange

            // Act
            var result = _controller.Search(new SearchViewModel());

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Properties.Should().HaveCount(2);
        }

        [Fact]
        public void SearchPost_NoPropertiesSpecified_NoPackagesFound()
        {
            // Arrange

            // Act
            var result = _controller.Search(new SearchViewModel());

            // Assert
            var model = (SearchViewModel) result.Model;
            model.Packages.Should().BeEmpty();
        }


        [Fact]
        public void Details_Simple_PackageInitialized()
        {
            // Arrange
            var package = _unitOfWork.GetRepository<ContentPackage>().Get().Last();
            var serviceMock = GetFinanceServiceMock();
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            // Act
            var result = _controller.Details(package.ID);

            // Assert
            var model = (DetailsViewModel) result.Model;
            model.Package.Should().Be(package);
        }

        [Fact]
        public void Details_Simple_PropertiesInitialized()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock();
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            // Act
            var result = _controller.Details(1);

            // Assert
            var model = (DetailsViewModel) result.Model;
            model.Properties.Should().HaveCount(2);
        }

        [Fact]
        public void Details_PermissionsNotGranted_LimitedAccess()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock(false);
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            // Act
            var result = _controller.Details(1);

            // Assert
            var model = (DetailsViewModel) result.Model;
            model.IsFullAccessGranted.Should().BeFalse();
        }

        [Fact]
        public void Details_UserIsAdmin_FullAccess()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock(false);
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext("admin");

            // Act
            var result = _controller.Details(1);

            // Assert
            var model = (DetailsViewModel) result.Model;
            model.IsFullAccessGranted.Should().BeTrue();
        }

        [Fact]
        public void Details_PermissionsGranted_FullAccess()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock(true);
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            // Act
            var result = _controller.Details(1);

            // Assert
            var model = (DetailsViewModel) result.Model;
            model.IsFullAccessGranted.Should().BeTrue();
        }

        [Fact]
        public void Details_PackageNotExists_ErrorViewShown()
        {
            // Arrange

            // Act
            var result = _controller.Details(-1);

            // Assert
            result.ViewName.Should().Be("ApplicationError");
        }


        [Fact]
        public void Download_FileNotExists_ErrorViewShown()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock();
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            // Act
            var result = _controller.Download(-1);

            // Assert
            var viewResult = (ViewResult) result;
            viewResult.ViewName.Should().Be("ApplicationError");
        }

        [Fact]
        public void Download_FileExists_FileStreamReturned()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock();
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            var contentFile = new ContentFile();
            _unitOfWork.GetRepository<ContentFile>().Insert(contentFile);
            _unitOfWork.Save();

            // Act
            _controller.Download(contentFile.ID);

            // Assert
            _fileStorageManager.GetOperationArgument.Should().Be(contentFile);
        }

        [Fact]
        public void Download_PermissionsNotGranted_ErrorMessageShown()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock(false);
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            var contentFile = new ContentFile();
            _unitOfWork.GetRepository<ContentFile>().Insert(contentFile);

            // Act
            var result = _controller.Download(contentFile.ID);

            // Assert
            var viewResult = (ViewResult) result;
            viewResult.ViewName.Should().Be("ApplicationError");
        }

        [Fact]
        public void Download_UserIsAdmin_PermissionsGranted()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock(false);
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext("admin");

            var contentFile = new ContentFile();
            _unitOfWork.GetRepository<ContentFile>().Insert(contentFile);
            _unitOfWork.Save();

            // Act
            var result = _controller.Download(contentFile.ID);

            // Assert
            result.Should().BeOfType<FileStreamResult>();
        }

        [Fact]
        public void Download_FileIsPreview_PermissionsGranted()
        {
            // Arrange
            var serviceMock = GetFinanceServiceMock(false);
            _controller = new ContentController(_unitOfWork, _fileStorageManager, serviceMock.Object);
            _controller.ControllerContext = CreateControllerContext();

            var contentFile = new ContentFile {IsPreview = true};
            _unitOfWork.GetRepository<ContentFile>().Insert(contentFile);
            _unitOfWork.Save();

            // Act
            var result = _controller.Download(contentFile.ID);

            // Assert
            result.Should().BeOfType<FileStreamResult>();
        }
    }
}