using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ContributionAccessLogDAO : BaseDAO<ContributionAccessLog>
    {
        private readonly ILogger<ContributionAccessLogDAO> _logger;

        public ContributionAccessLogDAO(AppDbContext context, ILogger<ContributionAccessLogDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<ContributionAccessLog> GetContributionAccessLogsQueryable()
        {
            return _dbSet                
                .AsQueryable();
        }
        public async Task<ContributionAccessLog?> GetContributionAccessLogs(int userId, int contributionId)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserId == userId && u.ContributionId == contributionId);
        }
    }
}
