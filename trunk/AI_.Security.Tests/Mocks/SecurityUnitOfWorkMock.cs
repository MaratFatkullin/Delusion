using System;
using AI_.Security.DAL;
using AI_.Security.Models;

namespace AI_.Security.Tests.Mocks
{
    public class SecurityUnitOfWorkMock : ISecurityUnitOfWork
    {
        private IRepository<User> _userRepository;

        #region ISecurityUnitOfWork Members

        public IRepository<User> UserRepository
        {
            get { return _userRepository ?? (_userRepository = new RepositoryMock<User>()); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}