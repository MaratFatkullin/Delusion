using System.IO;

namespace AI_.Studmix.Domain.DAL.FileSystem
{
    public interface IFileStorageProvider
    {
        void Write(string path, Stream inputStream);
        Stream Read(string path);
    }
}