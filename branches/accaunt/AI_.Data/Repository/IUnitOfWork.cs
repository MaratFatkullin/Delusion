using System;

namespace AI_.Data.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : ModelBase;
        void Save();
    }
}