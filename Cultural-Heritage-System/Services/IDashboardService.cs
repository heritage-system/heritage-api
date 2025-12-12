using Cultural_Heritage_System.Dtos.Response.Dashboard;

namespace Cultural_Heritage_System.Services
{
    public interface IDashboardService
    {
        Task<ExecutiveDashboardResponse> GetExecutiveDashboardAsync();
    }
}

