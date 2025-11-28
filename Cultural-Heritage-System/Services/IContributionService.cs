using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IContributionService
    {
        Task<ContributionResponse> PostContribution(ContributionCreationRequest request);
        Task<ContributionResponse> GetContributionDetail(int id);
        Task<ContributionOverviewResponse> GetContributionOverview(int id);
        Task<PageResponse<ContributionSearchResponse>> SearchContributionsAsync(ContributionSearchRequest request);
        Task<ContributionResponse> UnlockContribution(int contributionId);
        Task<bool> AddContributionSave(int contributionId);
        Task<PageResponse<ContributionSaveResponse>> GetContributionSave(ContributionSearchRequest request);      
        Task<bool> RemoveContributionSave(int contributionId);
        Task<List<TopContributionHeritageTagResponse>> GetTrendingContributionHeritageTag();
        Task<List<TrendingContributorDto>> GetTrendingContributor();
        Task<bool> CreateContributionReport(ContributionReportCreationRequest request);
        Task<List<ContributionSearchResponse>> GetContributionRelated(ContributionRelatedRequest request);
        Task<PageResponse<ContributionOverviewListItemResponse>> GetListContributionsOverview(ContributionOverviewSearchRequest request);
        Task<ContributionDetailUpdatedResponse> GetContributionDetailForUpdated(int id);
        Task<ContributionResponse> UpdateContribution(ContributionUpdateRequest request);
    }
}

