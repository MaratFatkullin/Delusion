using AI_.Data.Repository;
using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.ApplicationServices.Specifications
{
    public class GetRoleByRoleName : Specification<Role>
    {
        public GetRoleByRoleName(string rolename)
        {
            Filter = p => (p.RoleName == rolename.ToLower());
        }
    }
}