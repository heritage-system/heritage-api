
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.ContributionReport;

namespace Cultural_Heritage_System.Services.Impl
{
    public interface IContributionReportService
    {
        Task<PageResponse<ContributionReportResponse>> GetAllAsync(int page, int pageSize, string? keyword = null, DateTime? startDate = null, DateTime? endDate = null, string? status = null);
        Task<ContributionReportResponse?> GetByIdAsync(long id);   
        Task<bool> AnswerContributionReportAsync(long reportId, string answer);
    }
}


