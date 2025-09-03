using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services
{
    public interface IContributorService
    {
        //Task<PageResponse<ContributorResponse>> SearchContributorsAsync(ContributorSearchRequest request);
        Task<ContributorResponse> GetContributorDetail(int id);
        Task<ContributorResponse> CreateContributor(ContributorCreateRequest request);
        Task<ContributorResponse> UpdateContributor(int id, ContributorUpdateRequest request);
        Task DeleteContributor(int id);
    }
}
