using System;
using System.Data.Entity;

namespace AI_.Data.Repository
{
    public abstract class UnitOfWork<TContext>
        : IDisposable
        where TContext : DbContext, new()
    {
        private readonly TContext _context = new TContext();
        private bool _disposed;

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}