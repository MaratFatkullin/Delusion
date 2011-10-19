using System.Collections.Generic;
using System.Linq;
using System.Web;
using AI_.Studmix.WebApplication.ViewModels.Content;
using Xunit;
using FluentAssertions;

namespace AI_.Studmix.WebApplication.Tests.ViewModels
{
    public class UploadViewModelTests
    {
        [Fact]
        public void Validate_NoFilesPosted_ModelStateErrorRegistered()
        {
            // Arrange
            var viewModel = new UploadViewModel();
            viewModel.ContentFiles = new List<HttpPostedFileBase> { null };

            // Act
            var results = viewModel.Validate(null);

            // Assert
            results.Should().Contain(x => x.MemberNames.Contains("ContentFiles"));
        }
    }
}