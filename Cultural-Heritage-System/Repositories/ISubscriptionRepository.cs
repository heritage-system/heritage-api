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
    public interface ISubscriptionRepository : IBaseRepository<Subscription>
    {
        Task<Subscription?> GetActiveSubscription(int userId);
        IQueryable<Subscription> GetActiveSubscriptionQueryByUserId(int userId);

        Task<Subscription?> GetSubscriptionById(int id);
        Task<List<Subscription>> GetAllSubscriptionsByUserIdAsync(int userId);
        Task<IEnumerable<Subscription>> GetSubscriptionsAsync();
    }
}
