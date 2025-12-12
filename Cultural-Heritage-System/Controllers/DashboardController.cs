using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Dashboard;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            this.dashboardService = dashboardService;
        }

        [HttpGet("executive")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<ExecutiveDashboardResponse>> GetExecutiveDashboard()
        {
            var result = await dashboardService.GetExecutiveDashboardAsync();
            return new ApiResponse<ExecutiveDashboardResponse>
            {
                code = 200,
                message = "Dashboard stats fetched successfully",
                result = result
            };
        }
    }
}

