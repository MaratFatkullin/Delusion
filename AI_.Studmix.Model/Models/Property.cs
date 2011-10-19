using System.Collections.Generic;
using System.Linq;
using AI_.Data;
using AI_.Studmix.Model.DAL.Database;

namespace AI_.Studmix.Model.Models
{
    public class Property : ModelBase
    {
        public string Name { get; set; }

        public int Order { get; set; }

        public virtual ICollection<PropertyState> States { get; set; }

        public IEnumerable<PropertyState> GetBoundedStates(IUnitOfWork unitOfWork, PropertyState state)
        {
            var packages = state.GetBoundedPackages(unitOfWork);
            var propertyStates = packages.Aggregate(new List<PropertyState>().AsEnumerable(),
                                                    (acc, elem) => acc.Concat(elem.PropertyStates));

            return propertyStates.Where(st => st.Property.ID == ID)
                .Distinct(new DefaultModelEqualityComparer<PropertyState>());
        }
    }
}