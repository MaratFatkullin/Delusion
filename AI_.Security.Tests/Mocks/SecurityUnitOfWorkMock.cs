using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AI_.Data.Repository;
using AI_.Security.DAL;
using AI_.Security.Models;

namespace AI_.Security.Tests.Mocks
{
    public class SecurityUnitOfWorkMock : ISecurityUnitOfWork , IObservable<object>
    {
        private IRepository<User> _userRepository;
        private IRepository<Role> _roleRepository;

        private readonly ICollection<IObserver<object>> _observers;

        public SecurityUnitOfWorkMock()
        {
            _observers = new Collection<IObserver<object>>();
        }

        #region ISecurityUnitOfWork Members

        public IRepository<User> UserRepository
        {
            get { return _userRepository ?? (_userRepository = new RepositoryMock<User>(this)); }
        }

        public IRepository<Role> RoleRepository
        {
            get { return _roleRepository ?? (_roleRepository= new RepositoryMock<Role>(this)); }
        }

        public void Dispose()
        {
        }

        public void Save()
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(null);
            }
        }

        #endregion

        public IDisposable Subscribe(IObserver<object> observer)
        {
            _observers.Add(observer);
            return null;
        }
    }
}