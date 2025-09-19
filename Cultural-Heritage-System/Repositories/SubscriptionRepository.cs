using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories
{
    public class SubscriptionRepository : BaseRepository<Subscription>
    {
        private readonly ILogger<SubscriptionRepository> _logger;

        public SubscriptionRepository(AppDbContext context, ILogger<SubscriptionRepository> logger)
            : base(context)
        {
            _logger = logger;
        }
        public async Task<Subscription?> GetActiveSubscription(int userId)
        {
            var now = DateTime.UtcNow;

            return await _dbSet
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s => s.UserId == userId
                                       && s.Status == SubscriptionStatus.ACTIVE
                                       && s.StartAt <= now
                                       && s.EndAt >= now);
        }


    }
}
