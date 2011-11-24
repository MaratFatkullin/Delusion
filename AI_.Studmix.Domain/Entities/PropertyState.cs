using System.Collections.Generic;
using AI_.Data;

namespace AI_.Studmix.Domain.Entities
{
    public class PropertyState : AggregationRoot
    {
        public Property Property { get; protected set; }

        public string Value { get; protected set; }

        public int Index { get; protected set; }

        public ICollection<ContentPackage> ContentPackages { get; protected set; }

        public PropertyState(Property property, string value, int index)
        {
            Property = property;
            Value = value;
            Index = index;
        }

        public PropertyState()
        {
        }
    }
}