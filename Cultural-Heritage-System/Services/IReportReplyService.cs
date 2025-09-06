using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IReportReplyService
    {
        Task<IEnumerable<ReportReply>> GetAllAsync();
        Task<ReportReply> GetByIdAsync(long id);
        Task<IEnumerable<ReportReply>> GetRepliesByReportIdAsync(long reportId);
    }

}
