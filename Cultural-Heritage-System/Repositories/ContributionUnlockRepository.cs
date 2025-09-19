using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributionUnlockRepository : BaseRepository<ContributionUnlock>
    {
        private readonly ILogger<ContributionUnlockRepository> _logger;

        public ContributionUnlockRepository(AppDbContext context, ILogger<ContributionUnlockRepository> logger)
            : base(context)
        {
            _logger = logger;
        }
      
        public async Task<ContributionUnlock?> GetContributionUnlocks(int userId, int contributionId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserId == userId && u.ContributionId == contributionId);
        }
    }
}
