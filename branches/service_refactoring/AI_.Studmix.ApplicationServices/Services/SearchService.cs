using System;
using System.Collections.Generic;
using System.Linq;
using AI_.Data;
using AI_.Data.Repository;
using AI_.Studmix.ApplicationServices.Services.Abstractions;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.ApplicationServices.Services
{
    public class SearchService : DataAccessObject, ISearchService
    {
        public SearchService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public IEnumerable<PropertyState> GetBoundedStates(Property property, PropertyState state)
        {
            var packages = state.ContentPackages;
            var propertyStates = packages.Aggregate(new List<PropertyState>().AsEnumerable(),
                                                    (acc, elem) => acc.Concat(elem.PropertyStates));

            return propertyStates.Where(st => st.Property.ID == property.ID)
                .Distinct(new DefaultModelEqualityComparer<PropertyState>());
        }

        public IEnumerable<ContentPackage> FindPackageWithSamePropertyStates(
            IEnumerable<PropertyState> propertyStates)

        {
            if (propertyStates == null)
                throw new ArgumentNullException("propertyStates");

            IEnumerable<ContentPackage> contentPackages = UnitOfWork.GetRepository<ContentPackage>().Get();

            foreach (var propertyState in propertyStates)
            {
                contentPackages = contentPackages
                    .Where(p => p.PropertyStates.Any(ps => ps.ID == propertyState.ID));
            }
            return contentPackages;
        }
    }
}