using System.IO;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.DAL.FileSystem
{
    public interface IFileStorageManager
    {
        void Store(ContentFile file, Stream inputStream);
    }
}