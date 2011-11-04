using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services.Abstractions
{
    public interface IFinanceService
    {
        bool IsOrderAvailable(Order order);
        void MakeOrder(IUnitOfWork unitOfWork, Order order);
        bool UserHasPermissions(IUnitOfWork unitOfWork, User user, ContentPackage package);
        bool UserHasOrder(IUnitOfWork unitOfWork, User user, ContentPackage package);
    }
}