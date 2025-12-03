using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class SubscriptionPaymentRepository
    : BaseRepository<SubscriptionPayment>, ISubscriptionPaymentRepository
    {
        private readonly SubscriptionPaymentDAO _dao;

        public SubscriptionPaymentRepository(SubscriptionPaymentDAO dao) : base(dao)
        {
            _dao = dao;
        }

        public Task<SubscriptionPayment?> GetByOrderCodeAsync(long orderCode)
            => _dao.GetByOrderCodeAsync(orderCode);

        public Task<List<SubscriptionPayment>> GetBySubscriptionIdAsync(int subscriptionId)
            => _dao.GetBySubscriptionIdAsync(subscriptionId);
        
    }

}
