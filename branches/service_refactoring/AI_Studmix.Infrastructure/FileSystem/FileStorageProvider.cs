using System.Configuration;
using System.IO;

namespace AI_.Studmix.Domain.DAL.FileSystem
{
    public class FileStorageProvider : IFileStorageProvider
    {
        protected string FileStoragePath
        {
            get { return ConfigurationManager.AppSettings["FileStoragePath"]; }
        }

        public void Write(string path, Stream inputStream)
        {
            var fullPath = Path.Combine(FileStoragePath,path);
            var directoryName = Path.GetDirectoryName(fullPath);    
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            using (Stream file = File.OpenWrite(fullPath))
            {
                CopyStream(inputStream, file);
            }
            inputStream.Dispose();
        }

        public Stream Read(string path)
        {
            return File.OpenRead(Path.Combine(FileStoragePath,path));
        }

        private void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }
    }
}