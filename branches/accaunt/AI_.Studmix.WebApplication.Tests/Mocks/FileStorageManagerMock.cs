using System.IO;
using AI_.Studmix.Model.DAL.FileSystem;
using AI_.Studmix.Model.Models;
using Moq;

namespace AI_.Studmix.WebApplication.Tests.Mocks
{
    public class FileStorageManagerMock : IFileStorageManager
    {
        public void Store(ContentPackage package)
        {
            Package = package;
        }


        public Stream GetFileStream(ContentFile contentFile)
        {
            GetOperationArgument = contentFile;

            var streamMock = new Mock<Stream>().Object;
            return streamMock;
        }

        public ContentPackage Package { get; private set; }

        public ContentFile GetOperationArgument { get; private set; }
    }
}