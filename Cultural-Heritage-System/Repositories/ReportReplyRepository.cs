using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class ReportReplyRepository : BaseRepository<ReportReply>
    {
        private readonly ILogger<ReportReplyRepository> _logger;

        public ReportReplyRepository(AppDbContext context, ILogger<ReportReplyRepository> logger)
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
