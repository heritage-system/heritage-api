using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributionSaveRepository : BaseRepository<ContributionSave>
    {
        private readonly ILogger<ContributionSaveRepository> _logger;

        public ContributionSaveRepository(AppDbContext context, ILogger<ContributionSaveRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<ContributionSave> GetContributionSaveByUserAndContribution(int userId, long contributionId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ContributionId == contributionId);
        }

 

        public IQueryable<ContributionSave> GetContributionSavesQueryByUserId(int userId)
        {
            return _dbSet
                .Include(f => f.Contribution)            
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);
        }

        public async Task<bool> IsContributionSaveExists(int userId, long contributionId)
        {
            return await _dbSet
                .AnyAsync(f => f.UserId == userId && f.ContributionId == contributionId);
        }

        public async Task<int> GetContributionSaveCountByUserId(int userId)
        {
            return await _dbSet
                .CountAsync(f => f.UserId == userId);
        }
    }
}
