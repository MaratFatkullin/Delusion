using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services
{
    public class SearchService
    {
        public IEnumerable<ContentPackage> FindPackageWithSamePropertyStates(
            IUnitOfWork unitOfWork,
            IEnumerable<PropertyState> propertyStates)
        {
            if(propertyStates == null)
                throw new ArgumentNullException("propertyStates");

            if(propertyStates.Count() == 0)
                return new Collection<ContentPackage>();

            IEnumerable<ContentPackage> contentPackages = propertyStates.First().ContentPackages;

            foreach (var propertyState in propertyStates)
            {
                contentPackages = propertyState.ContentPackages.Where(contentPackages.Contains);
            }
            return contentPackages;
        }
    }
}