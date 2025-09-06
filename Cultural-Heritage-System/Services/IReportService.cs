using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Report;

namespace Cultural_Heritage_System.Services.Impl
{
    public interface IReportService
    {
        Task<PageResponse<ReportResponse>> GetAllAsync(int page, int pageSize, string? keyword = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<ReportResponse?> GetByIdAsync(long id);
        Task<ReportResponse> CreateAsync(CreateReportRequest request);
        Task<ReportResponse?> UpdateAsync(long id, UpdateReportRequest request);
        Task<bool> DeleteAsync(long id);
        Task<bool> AnswerReportAsync(long reportId, string answer);
    }
}


