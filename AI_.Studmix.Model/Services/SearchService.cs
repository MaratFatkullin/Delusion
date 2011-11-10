using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AI_.Data.Repository;
using AI_.Security.Services;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services
{
    public class SearchService : ServiceBase
    {
        public SearchService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IEnumerable<ContentPackage> FindPackageWithSamePropertyStates(
            IEnumerable<PropertyState> propertyStates)

        {
            if (propertyStates == null)
                throw new ArgumentNullException("propertyStates");

            if (propertyStates.Count() == 0)
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