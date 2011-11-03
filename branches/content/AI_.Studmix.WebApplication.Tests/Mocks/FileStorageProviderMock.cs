using System;
using System.Collections.Generic;
using System.IO;
using AI_.Studmix.Model.DAL.FileSystem;

namespace AI_.Studmix.WebApplication.Tests.Mocks
{
    public class FileStorageProviderMock : IFileStorageProvider
    {

        public List<string> Storage { get; set; }
        public List<string> FileData { get; set; }
        public string ReadOperationPathArgument { get; private set; }

        public FileStorageProviderMock()
        {
            Storage = new List<string>();
            FileData = new List<string>();
        }

        #region IFileStorageProvider Members

        public void Write(string path, Stream inputStream)
        {
            Storage.Add(path);
            using (var streamReader = new StreamReader(inputStream))
            {
                var text = streamReader.ReadToEnd();
                FileData.Add(text);
            }
        }

        public Stream Read(string path)
        {
            ReadOperationPathArgument = path;
            return null;
        }

        #endregion
    }
}