using System.Linq;
using AI_.Studmix.Domain.Entities;
using AI_.Studmix.Domain.Services.Abstractions;

namespace AI_.Studmix.Domain.Services
{
    public class FinanceService :IFinanceService
    {
        public bool IsOrderAvailable(Order order)
        {
            return order.ContentPackage.Price <= order.User.Balance;
        }

        public void MakeOrder(Order order)
        {
            var user = order.User;
            var package = order.ContentPackage;
            user.Orders.Add(order);

            package.Owner.Balance += package.Price;
            user.Balance -= package.Price;
        }
    }
}