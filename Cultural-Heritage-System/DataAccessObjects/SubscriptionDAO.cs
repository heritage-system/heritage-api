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

        public IQueryable<Subscription> GetActiveSubscriptionQueryByUserId(int userId)
        {
            return _dbSet
                .Include(f => f.Package)
                .Where(f => f.UserId == userId && f.Status == SubscriptionStatus.ACTIVE)
                .OrderBy(f => f.StartAt);
        }
        public async Task<Subscription?> GetSubscriptionById(int id)
        {
            return await _dbSet
                .Include(c => c.Package)             
                .FirstOrDefaultAsync(c => c.Id == id);
        }


        public async Task<List<Subscription>> GetSubscriptionsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(s => s.UserId == userId && s.Status == SubscriptionStatus.ACTIVE)
                .Include(s => s.Package)
                    .ThenInclude(p => p.PackageBenefits)
                    .ThenInclude(pb => pb.Benefit)
                .Include(s => s.Payments)
                .Include(s => s.UsageRecords)
                .OrderBy(s => s.StartAt)
                .ToListAsync();
        }


    }
}
