using System.Collections.Generic;
using System.Linq;
using AI_.Data;
using AI_.Studmix.WebApplication.DAL.Database;

namespace AI_.Studmix.WebApplication.Models
{
    public class PropertyState : ModelBase
    {
        public virtual Property Property { get; set; }

        public virtual ICollection<ContentPackage> ContentPackages { get; set; }

        public string Value { get; set; }


        public static PropertyState Get(IUnitOfWork unitOfWork,
                                        int propertyId,
                                        string stateValue)
        {
            return unitOfWork.PropertyStateRepository
                .Get(x => x.Property.ID == propertyId
                          && x.Value == stateValue)
                .FirstOrDefault();
        }

        public IEnumerable<ContentPackage> GetBoundedPackages(IUnitOfWork unitOfWork)
        {
            return unitOfWork.ContentPackageRepository
                .Get(x => x.PropertyStates.Any(ps => ps.ID == ID));
        }

        public IEnumerable<PropertyState> GetBoundedStates(IUnitOfWork unitOfWork,Property property)
        {
            return null;
        }
    }
}