using Cultural_Heritage_System.Dtos.Response.Dashboard;

namespace Cultural_Heritage_System.Services
{
    public interface IDashboardService
    {
        Task<ExecutiveDashboardResponse> GetExecutiveDashboardAsync();
        Task<List<HeritagePopularItem>> GetTopFavoritedHeritagesAsync(int top);
        Task<List<ContributionPopularItem>> GetTopViewedContributionsAsync(int top);
        Task<List<HeritageProvinceBucket>> GetHeritageByProvinceAsync();
        Task<List<ContributionTrendPoint>> GetContributionTrendAsync(int months);
        Task<List<EngagementByCategoryItem>> GetEngagementByCategoryAsync(int topCategories);
        Task<List<YearlyGrowthItem>> GetYearlyGrowthAsync(int years);
    }
}

