using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>
    {
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(AppDbContext context, ILogger<RefreshTokenRepository> logger)
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