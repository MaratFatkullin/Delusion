using AI_.Data.Repository;
using AI_.Security.Tests.Mocks;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.WebApplication.Tests.Mocks
{
    public class UnitOfWorkMock : SecurityUnitOfWorkMock, IUnitOfWork
    {
        private IRepository<ContentFile> _contentFileRepository;
        private IRepository<ContentPackage> _contentPackageRepository;
        private IRepository<Property> _propertyRepository;
        private IRepository<PropertyState> _propertyStateRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<UserProfile> _userProfileRepository;
       
        #region IUnitOfWork Members

        public IRepository<PropertyState> PropertyStateRepository
        {
            get
            {
                return _propertyStateRepository
                       ?? (_propertyStateRepository = new RepositoryMock<PropertyState>(this));
            }
        }

        public IRepository<Property> PropertyRepository
        {
            get { return _propertyRepository ?? (_propertyRepository = new RepositoryMock<Property>(this)); }
        }

        public IRepository<ContentFile> ContentFileRepository
        {
            get
            {
                return _contentFileRepository ??
                       (_contentFileRepository = new RepositoryMock<ContentFile>(this));
            }
        }

        public IRepository<ContentPackage> ContentPackageRepository
        {
            get
            {
                return _contentPackageRepository ??
                       (_contentPackageRepository = new RepositoryMock<ContentPackage>(this));
            }
        }

        public IRepository<UserProfile> UserProfileRepository
        {
            get
            {
                return _userProfileRepository ??
                       (_userProfileRepository = new RepositoryMock<UserProfile>(this));
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                return _orderRepository ??
                       (_orderRepository = new RepositoryMock<Order>(this));
            }
        }

        #endregion
    }
}