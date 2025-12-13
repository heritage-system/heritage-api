using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IContributionReportReplyRepository : IBaseRepository<ContributionReportReply>
    {
        Task<IEnumerable<ContributionReportReply>> GetRepliesByReportIdAsync(long reportId);
       
    }
}
