using AI_.Data.Repository;
using AI_.Data.Repository.Mocks;
using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.WebApplication.Tests.Mocks
{
    public class UnitOfWorkMock : IUnitOfWork
    {
        private IRepository<ContentFile> _contentFileRepository;
        private IRepository<ContentPackage> _contentPackageRepository;
        private IRepository<Property> _propertyRepository;
        private IRepository<PropertyState> _propertyStateRepository;
        
        private IRepository<Role> _roleRepository;
        private IRepository<User> _userRepository;

        #region IUnitOfWork Members

        public IRepository<User> UserRepository
        {
            get { return _userRepository ?? (_userRepository = new RepositoryMock<User>()); }
        }

        public IRepository<Role> RoleRepository
        {
            get { return _roleRepository ?? (_roleRepository = new RepositoryMock<Role>()); }
        }

        public IRepository<PropertyState> PropertyStateRepository
        {
            get { return _propertyStateRepository 
                ?? (_propertyStateRepository = new RepositoryMock<PropertyState>()); }

        }

        public IRepository<Property> PropertyRepository
        {
            get { return _propertyRepository ?? (_propertyRepository = new RepositoryMock<Property>()); }
        }

        public IRepository<ContentFile> ContentFileRepository
        {
            get
            {
                return _contentFileRepository ??
                       (_contentFileRepository = new RepositoryMock<ContentFile>());
            }
        }

        public IRepository<ContentPackage> ContentPackageRepository
        {
            get
            {
                return _contentPackageRepository ??
                       (_contentPackageRepository = new RepositoryMock<ContentPackage>());
            }
        }

        public void Dispose()
        {
        }

        public void Save()
        {
        }

        #endregion
    }
}