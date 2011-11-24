using System.Collections.Generic;
using System.IO;
using AI_.Studmix.ApplicationServices.DataTransferObjects;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.ApplicationServices.Services.Abstractions
{
    public interface IContentService
    {
        List<Property> GetProperties();

        void Store(ContentPackageDto package);
        
        //void Store(string contentPackage,
        //           int price,
        //           string description,
        //           Dictionary<int, string> states,
        //           Dictionary<string, Stream> contentFiles,
        //           Dictionary<string, Stream> previewContentFiles,
        //           User user);
    }
}