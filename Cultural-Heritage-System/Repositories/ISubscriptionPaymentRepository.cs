using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Repositories
{
    public interface ISubscriptionPaymentRepository : IBaseRepository<SubscriptionPayment>
    {
        Task<SubscriptionPayment?> GetByOrderCodeAsync(long orderCode);
        Task<List<SubscriptionPayment>> GetBySubscriptionIdAsync(int subscriptionId);
    }

}
