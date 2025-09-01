using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/contributors")]
    [ApiController]
    public class ContributorController : ControllerBase
    {
        private readonly IContributorService contributorService;

        public ContributorController(IContributorService contributorService)
        {
            this.contributorService = contributorService;
        }

        [HttpGet("search")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<PageResponse<ContributorResponse>>> SearchContributors([FromQuery] ContributorSearchRequest request)
        {
            var result = await contributorService.SearchContributorsAsync(request);
            return new ApiResponse<PageResponse<ContributorResponse>>(
                code: 200,
                message: "Contributors fetched successfully",
                result: result
            );
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<ContributorResponse>> GetDetail(int id)
        {
            var result = await contributorService.GetContributorDetail(id);
            return new ApiResponse<ContributorResponse>(
                code: 200,
                message: "Contributor details fetched successfully",
                result: result
            );
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<ContributorResponse>> Create([FromBody] ContributorCreateRequest request)
        {
            var result = await contributorService.CreateContributor(request);
            return new ApiResponse<ContributorResponse>(
                code: 201,
                message: "Contributor created successfully",
                result: result
            );
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<ContributorResponse>> Update(int id, [FromBody] ContributorUpdateRequest request)
        {
            var result = await contributorService.UpdateContributor(id, request);
            return new ApiResponse<ContributorResponse>(
                code: 200,
                message: "Contributor updated successfully",
                result: result
            );
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<object>> Delete(int id)
        {
            await contributorService.DeleteContributor(id);
            return new ApiResponse<object>(
                code: 200,
                message: "Contributor deleted successfully"
            );
        }
    }
}
