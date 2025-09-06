using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IContributionService
    {
        Task<ContributionResponse> PostContribution(ContributionCreationRequest request);
        Task<ContributionResponse> GetContributionDetail(int id);
        Task<PageResponse<ContributionSearchResponse>> SearchContributionsAsync(ContributionSearchRequest request);
    }
}

