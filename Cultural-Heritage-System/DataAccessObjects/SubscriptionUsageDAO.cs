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
    public class SubscriptionUsageDAO : BaseDAO<SubscriptionUsage>
    {
        private readonly ILogger<SubscriptionUsageDAO> _logger;

        public SubscriptionUsageDAO(AppDbContext context, ILogger<SubscriptionUsageDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
        public async Task<SubscriptionUsage?> GetActiveSubscriptionUsage(long id)
        {
            var now = DateTime.UtcNow;

            return await _dbSet           
                .FirstOrDefaultAsync(s => s.Id == id
                                       && s.Status == SubscriptionStatus.ACTIVE
                                       );
        }


    }
}
