using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ContributionAccessLogRepository : BaseRepository<ContributionAccessLog>
    {
        private readonly ILogger<ContributionAccessLogRepository> _logger;

        public ContributionAccessLogRepository(AppDbContext context, ILogger<ContributionAccessLogRepository> logger)
            : base(context)
        {
            _logger = logger;
        }
      
        public async Task<ContributionAccessLog?> GetContributionAccessLogs(int userId, int contributionId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserId == userId && u.ContributionId == contributionId);
        }
    }
}
