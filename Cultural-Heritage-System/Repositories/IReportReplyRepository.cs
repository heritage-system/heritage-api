using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IReportReplyRepository : IBaseRepository<ReportReply>
    {
        Task<IEnumerable<ReportReply>> GetRepliesByReportIdAsync(long reportId);
       
    }
}
