using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class SubscriptionRepository : BaseRepository<Subscription>, ISubscriptionRepository
    {
        private readonly SubscriptionDAO _entityDAO;

        public SubscriptionRepository(SubscriptionDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        public async Task<Subscription?> GetActiveSubscription(int userId)
        {         
            return await _entityDAO.GetActiveSubscription(userId);
        }

        

    }
}
