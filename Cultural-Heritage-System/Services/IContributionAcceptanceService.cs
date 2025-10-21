using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IContributionAcceptanceService
    {
        Task ApproveOrRejectAsync(long acceptanceId, ContributionStatus status, string? note = null);
        Task<ContributionOverviewResponse> GetContributionOverviewForStaff(int contributionId);
        Task<PageResponse<ContributionOverviewListItemResponse>> GetListContributionsOverviewForStaff(ContributionOverviewSearchRequest request);
    }

}
