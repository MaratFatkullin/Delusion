namespace AI_.Security.DAL
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public ISecurityUnitOfWork GetInstance()
        {
            return new SecurityUnitOfWork();
        }
    }
}