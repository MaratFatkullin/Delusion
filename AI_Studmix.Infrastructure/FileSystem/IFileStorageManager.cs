using System.IO;
using AI_.Studmix.Domain.Models;

namespace AI_.Studmix.Domain.DAL.FileSystem
{
    public interface IFileStorageManager
    {
        void Store(ContentPackage package);
        Stream GetFileStream(ContentFile contentFile);
    }
}