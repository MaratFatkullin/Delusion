using System;
using System.Data.Entity;
using System.Security;


namespace AI_.Security
{
    public class GenericUnitOfWork<TContext> : IDisposable
        where TContext : DbContext, new()
    {
        private readonly TContext _context = new TContext();
        private bool _disposed;

        public void Save()
        {
            _context.SaveChanges();
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
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}