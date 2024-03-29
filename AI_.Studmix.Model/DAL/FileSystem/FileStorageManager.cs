﻿using System.Collections.Generic;
using System.IO;
using AI_.Studmix.Model.Models;
using System.Linq;

namespace AI_.Studmix.Model.DAL.FileSystem
{
    public class FileStorageManager : IFileStorageManager
    {
        private const string DEFAULT_FOLDER_NAME = "-";
        protected IFileStorageProvider Provider { get; set; }

        public FileStorageManager(IFileStorageProvider fileStorageProvider)
        {
            Provider = fileStorageProvider;
        }

        public void Store(ContentPackage package)
        {
            var propertyStates = package.PropertyStates;
            package.Path = GetDirectoryPath(propertyStates);
            foreach (var file in package.Files)
            {
                var filePath = Path.Combine(package.Path, file.Name);
                Provider.Write(filePath, file.Stream);
            }
        }

        public Stream GetFileStream(ContentFile contentFile)
        {
            var path = Path.Combine(contentFile.ContentPackage.Path,contentFile.Name);
            return Provider.Read(path);
        }

        private string GetDirectoryPath(IEnumerable<PropertyState> propertyStates)
        {
            string path = string.Empty;
            if (propertyStates.Count() == 0)
                return path;
            var maxOrder = propertyStates.Max(ps=>ps.Property.Order);
            for (int i = 1; i <= maxOrder; i++)
            {
                var state = propertyStates.FirstOrDefault(ps => ps.Property.Order == i);
                var folderName = state == null
                                     ? DEFAULT_FOLDER_NAME
                                     : string.Format("{0}_{1}", state.Property.ID, state.Index);

                path = Path.Combine(path, folderName);
            }
            return path;
        }
    }
}