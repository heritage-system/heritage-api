using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ReportReplyRepository : BaseRepository<ReportReply>, IReportReplyRepository
    {
        private readonly ReportReplyDAO _entityDAO;

        public ReportReplyRepository(ReportReplyDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        public async Task<IEnumerable<ReportReply>> GetRepliesByReportIdAsync(long reportId)
        {
            return await _entityDAO.GetRepliesByReportIdAsync(reportId);
        }

    }
}
