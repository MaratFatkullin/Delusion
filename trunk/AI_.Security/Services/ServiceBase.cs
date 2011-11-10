using AI_.Data.Repository;

namespace AI_.Security.Services
{
    public abstract class ServiceBase
    {
        public IUnitOfWork UnitOfWork { get; set; }

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}