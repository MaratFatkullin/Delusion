using System;
using System.Collections.Generic;
using System.Linq;
using AI_.Data;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services
{
    public class PropertyStateService
    {
        public PropertyState GetState(IUnitOfWork unitOfWork,
                                      int propertyId,
                                      string stateValue)
        {
            return unitOfWork.PropertyStateRepository
                .Get(x => x.Property.ID == propertyId
                          && x.Value == stateValue)
                .FirstOrDefault();
        }

        public IEnumerable<PropertyState> GetBoundedStates(IUnitOfWork unitOfWork,
                                                           Property property,
                                                           PropertyState state)
        {
            var packages = state.ContentPackages;
            var propertyStates = packages.Aggregate(new List<PropertyState>().AsEnumerable(),
                                                    (acc, elem) => acc.Concat(elem.PropertyStates));

            return propertyStates.Where(st => st.Property.ID == property.ID)
                .Distinct(new DefaultModelEqualityComparer<PropertyState>());
        }

        public PropertyState CreateState(IUnitOfWork unitOfWork, Property property, string value)
        {
            var existingPropertyStates = property.States.Where(state => state.Value == value).FirstOrDefault();
            if (existingPropertyStates != null)
                throw new InvalidOperationException("Property state already exists.");

            int index = property.States.Count == 0 ? 1 : property.States.Max(x => x.Index) + 1;
            var propertyState = new PropertyState
                                {
                                    Property = property,
                                    Value = value,
                                    Index = index
                                };
            unitOfWork.PropertyStateRepository.Insert(propertyState);
            return propertyState;
        }
    }
}