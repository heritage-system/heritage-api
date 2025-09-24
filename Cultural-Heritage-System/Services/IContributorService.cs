using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contributor;

namespace Cultural_Heritage_System.Services
{
    public interface IContributorService
    {
        Task<PageResponse<ContributorResponse>> SearchContributorsAsync(ContributorSearchRequest request);
        Task<ContributorResponse> GetContributorDetail(int id);
        Task<ContributorResponse> CreateContributor(ContributorCreateRequest request);
        Task<ContributorResponse> UpdateContributor(int id, ContributorUpdateRequest request);
        Task DisableContributor(int id);
        Task<List<DropdownUserResponse>> SearchDropdownUserAsync(string? keyword);
        Task<ContributorResponse> ApproveContributor(int id);
        Task<ContributorResponse> RejectContributor(int id);
        Task<ContributorResponse> ApplyContributor(ContributorApplyRequest request);
        Task<ContributorApplyResponse?> GetContributorApplication();
        Task<ContributorResponse> ReActivateContributor(int id);
    }
}
