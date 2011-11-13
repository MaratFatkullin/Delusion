using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace AI_.Data.Repository
{
    public class UnitOfWork<TContext>
        : IUnitOfWork
        where TContext : DbContext, new()
    {
        protected TContext Context { get; private set; }
        private IDictionary<Type, object> Map { get; set; }

        private bool _disposed;

        public UnitOfWork()
        {
            Context = new TContext();
            Map = new Dictionary<Type, object>();
        }

        public IRepository<TEntity> GetRepository<TEntity>() 
            where TEntity : ModelBase
        {
            var type = typeof(TEntity);
            if (!Map.ContainsKey(type))
                Map.Add(type, new Repository<TEntity>(Context));
            return (IRepository<TEntity>)Map[type];
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}