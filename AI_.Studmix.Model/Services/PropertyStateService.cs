using System;
using System.Collections.Generic;
using System.Linq;
using AI_.Data;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services.Abstractions;

namespace AI_.Studmix.Model.Services
{
    public class PropertyStateService:ServiceBase
    {
        public PropertyStateService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public PropertyState GetState(int propertyId,string stateValue)
        {
            return UnitOfWork.PropertyStateRepository
                .Get(x => x.Property.ID == propertyId
                          && x.Value == stateValue)
                .FirstOrDefault();
        }

        public IEnumerable<PropertyState> GetBoundedStates(Property property,PropertyState state)
        {
            var packages = state.ContentPackages;
            var propertyStates = packages.Aggregate(new List<PropertyState>().AsEnumerable(),
                                                    (acc, elem) => acc.Concat(elem.PropertyStates));

            return propertyStates.Where(st => st.Property.ID == property.ID)
                .Distinct(new DefaultModelEqualityComparer<PropertyState>());
        }

        public PropertyState CreateState(Property property, string value)
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
            UnitOfWork.PropertyStateRepository.Insert(propertyState);
            UnitOfWork.Save();
            return propertyState;
        }
    }
}