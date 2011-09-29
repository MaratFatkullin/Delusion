using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AI_.Studmix.WebApplication.DAL.FileSystem;
using AI_.Studmix.WebApplication.Models;
using AI_.Studmix.WebApplication.Tests.Mocks;
using FluentAssertions;
using Xunit;

namespace AI_.Studmix.WebApplication.Tests
{
    public class FileStorageManagerTests
    {
        #region Utility Methods

        private static MemoryStream CreateInputStream(string data = "mockedData")
        {
            return new MemoryStream(Encoding.ASCII.GetBytes(data));
        }

        private ContentPackage CreateContent()
        {
            var file = new ContentFile {Name = "file1.txt"};
            var country = new Country {Name = "Country1"};
            var city = new City {Name = "City1", Country = country};
            country.Cities = new List<City> {city};

            var institute = new Institute
                            {
                                Name = "Institute1",
                                City = city,
                                Type = new InstituteType {Name = "InstituteType1"},
                            };

            var faculty = new Faculty {Name = "Faculty1"};

            var studingType = new StudingForm {Name = "StudingForm1"};

            var content = new ContentPackage
                          {
                              Course = new Course {Name = "Course1"},
                              Group = new Group {Name = "Group1"},
                              Faculty = faculty,
                              Institute = institute,
                              StudingForm = studingType,
                              Files = new List<ContentFile> {file}
                          };
            file.ContentPackage = content;

            return content;
        }

        #endregion

        [Fact]
        public void Store_Simple_FileSavedInNestedDirectory()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();

            // Act
            fileStorageManager.Store(content.Files.First(), CreateInputStream());

            // Assert
            fileStorageProviderMock.Storage.Should().Contain(
                @"storagePath\Country1\City1\InstituteType1\Institute1" +
                @"\StudingForm1\Faculty1\Course1\Group1\file1.txt");
        }

        [Fact]
        public void Store_Simple_FileContentWrited()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();

            // Act
            fileStorageManager.Store(content.Files.First(), CreateInputStream("MockedFileData"));

            // Assert
            fileStorageProviderMock.Storage.Should().Contain("MockedFileData");
        }
    }
}