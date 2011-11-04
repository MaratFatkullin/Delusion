using System.Linq;
using AI_.Security.Models;
using AI_.Studmix.Model.DAL.Database;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services.Abstractions;

namespace AI_.Studmix.Model.Services
{
    public class FinanceService : IFinanceService
    {
        public bool IsOrderAvailable(Order order)
        {
            return order.ContentPackage.Price <= order.UserProfile.Balance;
        }

        public void MakeOrder(IUnitOfWork unitOfWork, Order order)
        {
            unitOfWork.OrderRepository.Insert(order);
            var price = order.ContentPackage.Price;

            var membershipService = new MembershipService();
            var ownerProfile = membershipService.GetUserProfile(unitOfWork, order.ContentPackage.Owner);

            order.UserProfile.Balance -= price;
            ownerProfile.Balance += price;

            unitOfWork.Save();
        }

        public bool UserHasPermissions(IUnitOfWork unitOfWork, User user, ContentPackage package)
        {
            if (package.Owner.ID == user.ID)
                return true;

            return UserHasOrder(unitOfWork, user, package);
        }

        public bool UserHasOrder(IUnitOfWork unitOfWork, User user, ContentPackage package)
        {
            var membershipService = new MembershipService();
            var profile = membershipService.GetUserProfile(unitOfWork, user);
            var order = unitOfWork.OrderRepository.Get(o => o.ContentPackage.ID == package.ID
                                                             && o.UserProfile.ID == profile.ID)
                .FirstOrDefault();
            return order != null;
        }
    }
}