using System.IO;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.Domain
{
    public interface IFileStorageManager
    {
        void Store(ContentPackage package);
        Stream GetFileStream(ContentFile contentFile);
    }
}