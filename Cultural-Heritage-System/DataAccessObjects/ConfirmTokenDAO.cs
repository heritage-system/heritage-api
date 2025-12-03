using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ConfirmTokenDAO : BaseDAO<ConfirmToken>
    {
        private readonly ILogger<ConfirmTokenDAO> _logger;

        public ConfirmTokenDAO(AppDbContext context, ILogger<ConfirmTokenDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
      
        public async Task<ConfirmToken?> FindByTokenAsync(int userId, string requestToken)
        {
            return await _dbSet
            .Where(rt => rt.Token == requestToken
                && rt.UserId == userId
                && rt.Revoked == null
                && rt.Expires > DateTime.UtcNow)
            .FirstOrDefaultAsync();
        }

        public async Task<ConfirmToken?> FindTokenByUserIdAsync(int userId)
        {
            return await _dbSet
            .Where(rt => rt.UserId == userId)
            .FirstOrDefaultAsync();
        }
    }
}