using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class SubscriptionUsageRepository : BaseRepository<SubscriptionUsage>, ISubscriptionUsageRepository
    {
        private readonly SubscriptionUsageDAO _entityDAO;

        public SubscriptionUsageRepository(SubscriptionUsageDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        public async Task<SubscriptionUsage?> GetActiveSubscriptionUsage(long id)
        {         
            return await _entityDAO.GetActiveSubscriptionUsage(id);
        }


    }
}
