using AI_.Data.Repository;
using AI_.Security.DAL;
using AI_.Security.Models;

namespace AI_.Security.Tests.Mocks
{
    public class SecurityUnitOfWorkMock : ISecurityUnitOfWork
    {
        private IRepository<User> _userRepository;
        private IRepository<Role> _roleRepository;
        public bool IsDisposed { get; private set; }
        public bool IsSaved { get; private set; }

        public SecurityUnitOfWorkMock()
        {
            IsDisposed = false;
            IsSaved = false;
        }

        #region ISecurityUnitOfWork Members

        public IRepository<User> UserRepository
        {
            get { return _userRepository ?? (_userRepository = new RepositoryMock<User>()); }
        }

        public IRepository<Role> RoleRepository
        {
            get { return _roleRepository ?? (_roleRepository= new RepositoryMock<Role>()); }
        }

        public void Dispose()
        {
            Save();
            IsDisposed = true;
        }

        public void Save()
        {
            IsSaved = true;
        }

        #endregion
    }
}