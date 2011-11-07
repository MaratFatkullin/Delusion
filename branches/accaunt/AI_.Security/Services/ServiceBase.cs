
namespace AI_.Security.Services
{
    public abstract class ServiceBase <TUnitOfWork>
    {
        public TUnitOfWork UnitOfWork { get; private set; }

        protected ServiceBase(TUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}