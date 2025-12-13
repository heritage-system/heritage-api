using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributionReportReplyRepository : BaseRepository<ContributionReportReply>, IContributionReportReplyRepository
    {
        private readonly ContributionReportReplyDAO _entityDAO;

        public ContributionReportReplyRepository(ContributionReportReplyDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        public async Task<IEnumerable<ContributionReportReply>> GetRepliesByReportIdAsync(long reportId)
        {
            return await _entityDAO.GetRepliesByReportIdAsync(reportId);
        }

    }
}
