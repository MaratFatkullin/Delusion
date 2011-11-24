using System.Collections.Generic;
using System.Linq;
using System.Web;
using AI_.Studmix.ApplicationServices.DataTransferObjects;
using AI_.Studmix.ApplicationServices.Services.Abstractions;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.WebApplication.Controllers;
using AI_.Studmix.WebApplication.ViewModels.Content;
using FluentAssertions;
using Moq;
using Xunit;

namespace AI_.Studmix.WebApplication.Tests.Controllers
{
    public class ContentControllerTestFixture
    {
        protected Mock<IContentService> ContentService;
        protected Mock<IMembershipService> MembershipService;

        public ContentControllerTestFixture()
        {
            ContentService = new Mock<IContentService>();
            MembershipService = new Mock<IMembershipService>();
        }

        public ContentController CreateController()
        {
            return new ContentController(MembershipService.Object,ContentService.Object);
        }
    }


    public class ContentControllerTests : ContentControllerTestFixture
    {
        [Fact]
        public void Upload_Simple_PropertiesProvided()
        {
            // Arrange
            var properties = new List<Property>();
            ContentService.Setup(s => s.GetProperties()).Returns(properties);

            // Act
            var contentController = CreateController();
            var result = contentController.Upload();

            // Assert
            var model = (UploadViewModel) result.Model;
            model.Properties.Should().Equal(properties);
        }

        [Fact]
        public void UploadPost_Simple_PropertiesProvided()
        {
            // Arrange
            ContentPackageDto actual = null;
            ContentService.Setup(s => s.Store(It.IsAny<ContentPackageDto>()))
                .Callback((ContentPackageDto arg) => actual = arg);
            var user = new User();
            MembershipService.Setup(s => s.GetUser(It.IsAny<string>())).Returns(user);

            var viewModel = new UploadViewModel
                            {
                                Caption = "caption",
                                Price = 100,
                                States = new Dictionary<int, string> {{1, "value"}},
                                Description = "description",
                                ContentFiles = new List<HttpPostedFileBase>(),
                                PreviewContentFiles = new List<HttpPostedFileBase>()
                            };

            // Act
            var contentController = CreateController();
            contentController.Upload(viewModel);

            // Assert
            actual.ShouldHave().SharedProperties().EqualTo(viewModel);
        }
    }
}