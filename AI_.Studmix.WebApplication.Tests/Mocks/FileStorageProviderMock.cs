using System.Collections.Generic;
using System.IO;
using AI_.Studmix.WebApplication.DAL.FileSystem;

namespace AI_.Studmix.WebApplication.Tests.Mocks
{
    public class FileStorageProviderMock : IFileStorageProvider
    {
        private string _storagePath = "storagePath";

        public List<string> Storage { get; set; }

        public string StoragePath
        {
            get { return _storagePath; }
            set { _storagePath = value; }
        }

        public FileStorageProviderMock()
        {
            Storage = new List<string>();
        }

        #region IFileStorageProvider Members

        public void Write(string path, Stream inputStream)
        {
            Storage.Add(Path.Combine(StoragePath, path));
            using (var streamReader = new StreamReader(inputStream))
            {
                string text = streamReader.ReadToEnd();
                Storage.Add(text);
            }
        }

        #endregion
    }
}