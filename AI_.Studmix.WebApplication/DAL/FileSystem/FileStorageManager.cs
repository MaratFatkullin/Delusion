using System.IO;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.DAL.FileSystem
{
    public class FileStorageManager : IFileStorageManager
    {
        protected IFileStorageProvider FileStorageProvider { get; set; }

        public FileStorageManager(IFileStorageProvider fileStorageProvider)
        {
            FileStorageProvider = fileStorageProvider;
        }

        public void Store(ContentFile file, Stream inputStream)
        {
            var path = Path.Combine(
                file.ContentPackage.Institute.City.Country.Name,
                file.ContentPackage.Institute.City.Name,
                file.ContentPackage.Institute.Type.Name,
                file.ContentPackage.Institute.Name,
                file.ContentPackage.StudingForm.Name,
                file.ContentPackage.Faculty.Name,
                file.ContentPackage.Course.Name,
                file.ContentPackage.Group.Name,
                file.Name);
            FileStorageProvider.Write(path, inputStream);
        }
    }
}