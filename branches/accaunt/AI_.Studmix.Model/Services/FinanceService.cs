using System.Linq;
using AI_.Data.Repository;
using AI_.Security.Models;
using AI_.Security.Services;
using AI_.Studmix.Model.Models;
using AI_.Studmix.Model.Services.Abstractions;

namespace AI_.Studmix.Model.Services
{
    public class FinanceService : ServiceBase<IUnitOfWork>, IFinanceService
    {
        public FinanceService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        #region IFinanceService Members

        public bool IsOrderAvailable(Order order)
        {
            return order.ContentPackage.Price <= order.UserProfile.Balance;
        }

        public void MakeOrder(Order order)
        {
            UnitOfWork.GetRepository<Order>().Insert(order);
            var price = order.ContentPackage.Price;

            var membershipService = new MembershipService(UnitOfWork);
            var ownerProfile = membershipService.GetUserProfile(order.ContentPackage.Owner);

            order.UserProfile.Balance -= price;
            ownerProfile.Balance += price;

            UnitOfWork.Save();
        }

        public bool UserHasPermissions(User user, ContentPackage package)
        {
            if (package.Owner.ID == user.ID)
                return true;

            return UserHasOrder(user, package);
        }

        public bool UserHasOrder(User user, ContentPackage package)
        {
            var membershipService = new MembershipService(UnitOfWork);
            var profile = membershipService.GetUserProfile(user);
            var order = UnitOfWork.GetRepository<Order>().Get(o => o.ContentPackage.ID == package.ID
                                                            && o.UserProfile.ID == profile.ID)
                .FirstOrDefault();
            return order != null;
        }

        #endregion
    }
}