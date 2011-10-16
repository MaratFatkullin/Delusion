using AI_.Studmix.WebApplication.DAL.FileSystem;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.Tests.Mocks
{
    public class FileStorageManagerMock : IFileStorageManager
    {
        public void Store(ContentPackage package)
        {
            Package = package;
        }

        public ContentPackage Package { get; private set; }
    }
}