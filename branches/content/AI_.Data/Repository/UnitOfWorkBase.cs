using System;
using System.Data.Entity;

namespace AI_.Data.Repository
{
    public abstract class UnitOfWorkBase<TContext>
        : IDisposable
        where TContext : DbContext, new()
    {
        protected TContext Context { get; private set; }

        private bool _disposed;

        protected UnitOfWorkBase()
        {
            Context = new TContext();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void Save()
        {
            Context.SaveChanges();
        }

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