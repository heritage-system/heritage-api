using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ContributionUnlockDAO : BaseDAO<ContributionUnlock>
    {
        private readonly ILogger<ContributionUnlockDAO> _logger;

        public ContributionUnlockDAO(AppDbContext context, ILogger<ContributionUnlockDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<ContributionUnlock> GetContributionUnlockByUserAndContribution(int userId, long contributionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ContributionId == contributionId);
        }

  


        public IQueryable<ContributionUnlock> GetContributionUnlocksQueryByUserId(int userId)
        {
            return _dbSet
                .Include(f => f.Contribution)            
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);
        }

        public async Task<bool> IsContributionUnlockExists(int userId, long contributionId)
        {
            return await _dbSet
                .AnyAsync(f => f.UserId == userId && f.ContributionId == contributionId);
        }

        public async Task<int> GetContributionUnlockCountByUserId(int userId)
        {
            return await _dbSet
                .CountAsync(f => f.UserId == userId);
        }
    }
}
