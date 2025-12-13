using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class ReportReplyDAO : BaseDAO<ReportReply>
    {
        private readonly ILogger<ReportReplyDAO> _logger;

        public ReportReplyDAO(AppDbContext context, ILogger<ReportReplyDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
        public async Task<IEnumerable<ReportReply>> GetRepliesByReportIdAsync(long reportId)
        {
            return await _dbSet
                .Where(r => r.ReportId == reportId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

    }
}
