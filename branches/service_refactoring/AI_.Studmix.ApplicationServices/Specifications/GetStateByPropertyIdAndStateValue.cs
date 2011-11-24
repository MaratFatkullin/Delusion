using AI_.Data.Repository;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.ApplicationServices.Specifications
{
    public class GetStateByPropertyIdAndStateValue : Specification<PropertyState>
    {
        public GetStateByPropertyIdAndStateValue(int propertyId, string stateValue)
        {
            Filter = p => (p.ID == propertyId && p.Value == stateValue);
        }
    }
}