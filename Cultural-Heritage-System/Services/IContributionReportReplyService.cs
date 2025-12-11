using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IContributionReportReplyService
    {
        Task<IEnumerable<ContributionReportReply>> GetAllAsync();
        Task<ContributionReportReply> GetByIdAsync(long id);
        Task<IEnumerable<ContributionReportReply>> GetRepliesByContributionReportIdAsync(long reportId);
    }

}
