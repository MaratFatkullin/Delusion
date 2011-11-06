using AI_.Studmix.Model.DAL.Database;

namespace AI_.Studmix.Model.Services.Abstractions
{
    public class ServiceBase
    {
        public IUnitOfWork UnitOfWork { get; private set; }

        public ServiceBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}