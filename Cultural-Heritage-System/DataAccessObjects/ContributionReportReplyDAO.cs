using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ContributionReportReplyDAO : BaseDAO<ContributionReportReply>
    {
        private readonly ILogger<ContributionReportReplyDAO> _logger;

        public ContributionReportReplyDAO(AppDbContext context, ILogger<ContributionReportReplyDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<ContributionReportReply>> GetRepliesByReportIdAsync(long reportId)
        {
            return await _dbSet
                .Where(r => r.ContributionReportId == reportId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

    }
}
