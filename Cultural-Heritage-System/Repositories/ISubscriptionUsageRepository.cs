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
    public interface ISubscriptionUsageRepository : IBaseRepository<SubscriptionUsage>
    {
        Task<SubscriptionUsage?> GetActiveSubscriptionUsage(long id);
        Task<bool> ExistsAsync(int subscriptionId, string benefitName);
    }
}
