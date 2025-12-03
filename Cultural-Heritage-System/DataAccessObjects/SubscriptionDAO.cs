using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class SubscriptionDAO : BaseDAO<Subscription>
    {
        private readonly ILogger<SubscriptionDAO> _logger;

        public SubscriptionDAO(AppDbContext context, ILogger<SubscriptionDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
        public async Task<Subscription?> GetActiveSubscription(int userId)
        {
            var now = DateTime.UtcNow;

            return await _dbSet
                .Include(s => s.Package)
                .Include(s => s.UsageRecords)
                .Include(s=> s.Payments)
                .FirstOrDefaultAsync(s => s.UserId == userId
                                       && s.Status == SubscriptionStatus.ACTIVE
                                       && s.StartAt <= now
                                       && s.EndAt >= now);
        }

    }
}
