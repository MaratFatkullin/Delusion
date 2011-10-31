using System.IO;

namespace AI_.Studmix.Model.DAL.FileSystem
{
    public interface IFileStorageProvider
    {
        void Write(string path, Stream inputStream);
    }
}