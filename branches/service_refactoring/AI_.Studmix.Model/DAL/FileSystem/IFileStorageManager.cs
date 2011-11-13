using System.IO;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.DAL.FileSystem
{
    public interface IFileStorageManager
    {
        void Store(ContentPackage package);
        Stream GetFileStream(ContentFile contentFile);
    }
}