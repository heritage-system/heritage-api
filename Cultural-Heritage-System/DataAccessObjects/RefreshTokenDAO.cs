using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class RefreshTokenDAO : BaseDAO<RefreshToken>
    {
        private readonly ILogger<RefreshTokenDAO> _logger;

        public RefreshTokenDAO(AppDbContext context, ILogger<RefreshTokenDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
      
        public async Task<RefreshToken?> FindByTokenAsync(string requestToken)
        {
            return await _dbSet
            .Where(rt => rt.Token == requestToken
                && rt.Revoked == null
                && rt.Expires > DateTime.UtcNow)
            .FirstOrDefaultAsync();
        }
    }
}