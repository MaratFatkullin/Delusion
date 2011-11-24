using AI_.Studmix.Domain.Entities;

namespace AI_.Studmix.Domain.Services.Abstractions
{
    public interface IFinanceService
    {
        bool IsOrderAvailable(Order order);
        void MakeOrder(Order order);
    }
}