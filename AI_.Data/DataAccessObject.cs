using AI_.Data.Repository;

namespace AI_.Data
{
    public abstract class DataAccessObject
    {
        public IUnitOfWork UnitOfWork { get; protected set; }

        protected DataAccessObject(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}