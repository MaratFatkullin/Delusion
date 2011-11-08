using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AI_.Data;
using AI_.Data.Repository;

namespace AI_.Security.Tests.Mocks
{
    public class UnitOfWorkMock : IUnitOfWork, IObservable<object>
    {
        private readonly ICollection<IObserver<object>> _observers;
        private IDictionary<Type, object> Map { get; set; }

        public UnitOfWorkMock()
        {
            _observers = new Collection<IObserver<object>>();
        }

        #region IObservable<object> Members

        public IDisposable Subscribe(IObserver<object> observer)
        {
            _observers.Add(observer);
            return null;
        }

        #endregion

        #region IUnitOfWork Members

        public void Dispose()
        {
        }

        public IRepository<TEntity> GetRepository<TEntity>()
            where TEntity : ModelBase
        {
            var type = typeof (TEntity);
            if (!Map.ContainsKey(type))
                Map.Add(type, new RepositoryMock<TEntity>(this));
            return (IRepository<TEntity>) Map[type];
        }

        public void Save()
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(null);
            }
        }

        #endregion
    }
}