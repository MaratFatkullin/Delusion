using AI_.Security.DAL;

namespace AI_.Studmix.WebApplication.DAL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public ISecurityUnitOfWork GetInstance()
        {
            return new UnitOfWork();
        }
    }
}