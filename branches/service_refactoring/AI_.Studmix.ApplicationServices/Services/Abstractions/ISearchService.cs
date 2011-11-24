using System.Collections.Generic;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.ApplicationServices.Services.Abstractions
{
    public interface ISearchService
    {
        IEnumerable<PropertyState> GetBoundedStates(Property property, PropertyState state);

        IEnumerable<ContentPackage> FindPackageWithSamePropertyStates(
            IEnumerable<PropertyState> propertyStates);
    }
}