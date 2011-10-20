using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using AI_.Studmix.Model.DAL.FileSystem;
using AI_.Studmix.Model.Models;
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
            var file1 = new ContentFile {Name = "file1.txt", Stream = CreateInputStream()};
            var file2 = new ContentFile {Name = "file2.txt", Stream = CreateInputStream()};
            var content = new ContentPackage {Files = new List<ContentFile> {file1, file2}};

            file1.ContentPackage = content;
            file2.ContentPackage = content;

            var property1 = new Property {ID = 1, Order = 1};
            var property2 = new Property {ID = 2, Order = 2};
            var state1 = new PropertyState {ID = 1, Value = "prop1", Property = property1,Index = 1};
            var state2 = new PropertyState {ID = 2, Value = "prop2", Property = property2,Index = 2};

            content.PropertyStates = new Collection<PropertyState> {state1, state2};

            return content;
        }

        #endregion

        [Fact]
        public void Store_ThereIsTwoFilesInPackage_FirstFileSaved()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();

            // Act
            fileStorageManager.Store(content);

            // Assert
            fileStorageProviderMock.Storage.Should().Contain(@"1_1\2_2\file1.txt");
        }

        [Fact]
        public void Store_ThereIsTwoFilesInPackage_SecondFileSaved()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();

            // Act
            fileStorageManager.Store(content);

            // Assert
            fileStorageProviderMock.Storage.Should().Contain(@"1_1\2_2\file2.txt");
        }

        [Fact]
        public void Store_Simple_FilePathContainPropertyStateIndex()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();
            content.PropertyStates.Last().Index = 5;

            // Act
            fileStorageManager.Store(content);

            // Assert
            fileStorageProviderMock.Storage.Should().Contain(@"1_1\2_5\file2.txt");
        }

        [Fact]
        public void Store_ThereIsUnspecifiedPropertyInPackage_FilePathContainsDefaultNamedFolder()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();
            content.PropertyStates.Last().Property.Order = 3;

            // Act
            fileStorageManager.Store(content);

            // Assert
            fileStorageProviderMock.Storage.Should().Contain(@"1_1\-\2_2\file1.txt");
        }


        [Fact]
        public void Store_PackagePropertyStatesNotOrdered_FilePathCombinedFromOrderedStates()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();
            content.PropertyStates = content.PropertyStates.Reverse().ToList();

            // Act
            fileStorageManager.Store(content);

            // Assert
            fileStorageProviderMock.Storage.Should().Contain(@"1_1\2_2\file1.txt");
        }

        [Fact]
        public void Store_Simple_FileContentWrited()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();
            content.Files.First().Stream = CreateInputStream("MockedFileData");

            // Act
            fileStorageManager.Store(content);

            // Assert
            fileStorageProviderMock.FileData.Should().Contain("MockedFileData");
        }

        [Fact]
        public void Store_NoPropertySpecified_FileContentWritedToRootDirectory()
        {
            // Arrange
            var fileStorageProviderMock = new FileStorageProviderMock();
            var fileStorageManager = new FileStorageManager(fileStorageProviderMock);
            var content = CreateContent();
            content.PropertyStates = new Collection<PropertyState>();

            // Act
            fileStorageManager.Store(content);

            // Assert
            fileStorageProviderMock.Storage.Should().Contain(@"file1.txt");
        }
    }
}