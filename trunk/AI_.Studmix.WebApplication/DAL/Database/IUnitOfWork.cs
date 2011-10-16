using AI_.Data.Repository;
using AI_.Security.DAL;
using AI_.Studmix.WebApplication.Models;

namespace AI_.Studmix.WebApplication.DAL.Database
{
    public interface IUnitOfWork : ISecurityUnitOfWork
    {
        IRepository<PropertyState> PropertyStateRepository { get; }
        IRepository<Property> PropertyRepository { get; }
        IRepository<ContentFile> ContentFileRepository { get; }
        IRepository<ContentPackage> ContentPackageRepository { get; }

    }
}