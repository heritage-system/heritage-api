using Cultural_Heritage_System.Dtos.Request.Subscription;
using Cultural_Heritage_System.Dtos.Response.Subscription;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface ISubscriptionService
    {
        Task<CreatePaymentResponse> CreateSubscriptionAsync(CreateSubscriptionRequest request);
        Task<bool> CheckPaymentStatusAsync(long orderCode);
        Task<Subscription?> GetActiveSubscriptionAsync();
        Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId);
        Task<bool> CancelSubscriptionAsync(int userId, int subscriptionId);
        Task<SubscriptionPayment?> GetPaymentByOrderCodeAsync(long orderCode);
        Task<List<SubscriptionResponse>> GetSubscriptionsByUserIdAsync();
        Task<IEnumerable<SubscriptionResponse>> GetAllSubscriptionsAsync();
    }

}
