using System;
using System.Collections.Generic;
using System.Linq;
using AI_.Data;

namespace AI_.Studmix.Domain.Entities
{
    public class Property : AggregationRoot
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public virtual ICollection<PropertyState> States { get; set; }

        public PropertyState AddState(string value)
        {
            var existingPropertyStates = States.Where(state => state.Value == value).FirstOrDefault();
            if (existingPropertyStates != null)
                throw new InvalidOperationException("Property state already exists.");

            int index = States.Count == 0 ? 1 : States.Max(x => x.Index) + 1;
            var propertyState = new PropertyState(this, value, index);

            States.Add(propertyState);
            return propertyState;
        }
    }
}