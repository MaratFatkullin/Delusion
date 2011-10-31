using AI_.Data.Repository;
using AI_.Security.DAL;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.DAL.Database
{
    public interface IUnitOfWork : ISecurityUnitOfWork
    {
        IRepository<PropertyState> PropertyStateRepository { get; }
        IRepository<Property> PropertyRepository { get; }
        IRepository<ContentFile> ContentFileRepository { get; }
        IRepository<ContentPackage> ContentPackageRepository { get; }
        IRepository<UserProfile> UserProfileRepository { get;}
        IRepository<Purchase> PurchaseRepository { get; }
    }
}