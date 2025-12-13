using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class SubscriptionPaymentDAO : BaseDAO<SubscriptionPayment>
    {
        private readonly ILogger<SubscriptionPaymentDAO> _logger;

        public SubscriptionPaymentDAO(AppDbContext context, ILogger<SubscriptionPaymentDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<SubscriptionPayment?> GetByOrderCodeAsync(long orderCode)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

        public async Task<List<SubscriptionPayment>> GetBySubscriptionIdAsync(int subscriptionId)
        {
            return await _dbSet
                .Where(p => p.SubscriptionId == subscriptionId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
