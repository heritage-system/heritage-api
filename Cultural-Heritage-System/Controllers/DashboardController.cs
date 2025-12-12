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

        [HttpGet("heritage/top-viewed")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<List<HeritagePopularItem>>> GetTopViewedHeritage([FromQuery] int top = 5)
        {
            var data = await dashboardService.GetTopFavoritedHeritagesAsync(top);
            return new ApiResponse<List<HeritagePopularItem>>
            {
                code = 200,
                message = "Top favorited heritages fetched successfully",
                result = data
            };
        }

        [HttpGet("contributions/top-viewed")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<List<ContributionPopularItem>>> GetTopViewedContributions([FromQuery] int top = 5)
        {
            var data = await dashboardService.GetTopViewedContributionsAsync(top);
            return new ApiResponse<List<ContributionPopularItem>>
            {
                code = 200,
                message = "Top viewed contributions fetched successfully",
                result = data
            };
        }

        [HttpGet("heritage/by-province")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<List<HeritageProvinceBucket>>> GetHeritageByProvince()
        {
            var data = await dashboardService.GetHeritageByProvinceAsync();
            return new ApiResponse<List<HeritageProvinceBucket>>
            {
                code = 200,
                message = "Heritage distribution by province fetched successfully",
                result = data
            };
        }

        [HttpGet("contributions/trend")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<List<ContributionTrendPoint>>> GetContributionTrend([FromQuery] int months = 6)
        {
            var data = await dashboardService.GetContributionTrendAsync(months);
            return new ApiResponse<List<ContributionTrendPoint>>
            {
                code = 200,
                message = "Contribution trend fetched successfully",
                result = data
            };
        }

        [HttpGet("engagement/by-category")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<List<EngagementByCategoryItem>>> GetEngagementByCategory([FromQuery] int top = 6)
        {
            var data = await dashboardService.GetEngagementByCategoryAsync(top);
            return new ApiResponse<List<EngagementByCategoryItem>>
            {
                code = 200,
                message = "Engagement by category fetched successfully",
                result = data
            };
        }

        [HttpGet("growth/yearly")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<List<YearlyGrowthItem>>> GetYearlyGrowth([FromQuery] int years = 6)
        {
            var data = await dashboardService.GetYearlyGrowthAsync(years);
            return new ApiResponse<List<YearlyGrowthItem>>
            {
                code = 200,
                message = "Yearly growth fetched successfully",
                result = data
            };
        }
    }
}

