using System;
using AI_.Data.Repository;

namespace AI_.Security.Services
{
    public abstract class ServiceBase : IDisposable
    {
        protected IUnitOfWork UnitOfWork { get; private set; }

        protected ServiceBase(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public void Save()
        {
            UnitOfWork.Save();
        }

        public void Dispose()
        {
            UnitOfWork.Dispose();
        }
    }
}