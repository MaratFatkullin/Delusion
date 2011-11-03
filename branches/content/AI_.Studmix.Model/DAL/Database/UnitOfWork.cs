using AI_.Data.Repository;
using AI_.Security.DAL;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.DAL.Database
{
    public class UnitOfWork : SecurityUnitOfWork<DataContext>, IUnitOfWork
    {
        private IRepository<PropertyState> _propertyStateRepository;
        private IRepository<Property> _propertyRepository;
        private IRepository<ContentFile> _contentFileRepository;
        private IRepository<ContentPackage> _contentPackageRepository;
        private IRepository<UserProfile> _userProfileRepository;
        private IRepository<Order> _orderRepository;

        #region IUnitOfWork Members

        public IRepository<Property> PropertyRepository
        {
            get
            {
                return _propertyRepository
                       ?? (_propertyRepository = new Repository<Property>(Context));
            }
        }

        public IRepository<PropertyState> PropertyStateRepository
        {
            get
            {
                return _propertyStateRepository
                       ?? (_propertyStateRepository = new Repository<PropertyState>(Context));
            }
        }

        public IRepository<ContentFile> ContentFileRepository
        {
            get
            {
                return _contentFileRepository
                       ?? (_contentFileRepository = new Repository<ContentFile>(Context));
            }
        }

        public IRepository<ContentPackage> ContentPackageRepository
        {
            get
            {
                return _contentPackageRepository
                       ?? (_contentPackageRepository = new Repository<ContentPackage>(Context));
            }
        }

        public IRepository<UserProfile> UserProfileRepository
        {
            get { return _userProfileRepository 
                ?? (_userProfileRepository = new Repository<UserProfile>(Context)); }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                return _orderRepository
                    ?? (_orderRepository = new Repository<Order>(Context));
            }
        }

        #endregion
    }
}