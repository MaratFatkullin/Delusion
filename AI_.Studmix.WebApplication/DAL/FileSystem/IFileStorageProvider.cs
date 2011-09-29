using System.IO;

namespace AI_.Studmix.WebApplication.DAL.FileSystem
{
    public interface IFileStorageProvider
    {
        void Write(string path, Stream inputStream);
    }
}