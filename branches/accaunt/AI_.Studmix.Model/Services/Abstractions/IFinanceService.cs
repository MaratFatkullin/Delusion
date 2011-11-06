using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;

namespace AI_.Studmix.Model.Services.Abstractions
{
    public interface IFinanceService
    {
        bool IsOrderAvailable(Order order);
        void MakeOrder(Order order);
        bool UserHasPermissions(User user, ContentPackage package);
        bool UserHasOrder(User user, ContentPackage package);
    }
}