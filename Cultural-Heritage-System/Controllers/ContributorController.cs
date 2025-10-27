using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contributor;
using Cultural_Heritage_System.Models;
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
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<PageResponse<ContributorResponse>>> SearchContributors([FromQuery] ContributorSearchRequest request)
        {
            return new ApiResponse<PageResponse<ContributorResponse>>(200, "Search Contributors",
                await contributorService.SearchContributorsAsync(request));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,STAFF")]
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
        [Authorize(Roles = "ADMIN,STAFF")]
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
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<ContributorResponse>> Update(int id, [FromBody] ContributorUpdateRequest request)
        {
            var result = await contributorService.UpdateContributor(id, request);
            return new ApiResponse<ContributorResponse>(
                code: 200,
                message: "Contributor updated successfully",
                result: result
            );
        }

        [HttpPut("{id}/disable")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<object>> Disable(int id)
        {
            await contributorService.DisableContributor(id);
            return new ApiResponse<object>(
                code: 200,
                message: "Contributor disabled successfully"
            );
        }

        [HttpGet("dropdown-users")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<List<DropdownUserResponse>>> SearchDropdownUser([FromQuery] string? keyword)
        {
            var result = await contributorService.SearchDropdownUserAsync(keyword);
            return new ApiResponse<List<DropdownUserResponse>>(200,"List of members",result );
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<ContributorResponse>> Approve(int id)
        {
            var result = await contributorService.ApproveContributor(id);
            return new ApiResponse<ContributorResponse>(
                200,
                "Contributor approved successfully",
                result
            );
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<ContributorResponse>> Reject(int id)
        {
            var result = await contributorService.RejectContributor(id);
            return new ApiResponse<ContributorResponse>(
                200,
                "Contributor rejected successfully",
                result
            );
        }

        [HttpPost("apply")]
        [Authorize(Roles = "MEMBER, CONTRIBUTOR")]
        public async Task<ApiResponse<ContributorResponse>> Apply([FromBody] ContributorApplyRequest request)
        {
            var result = await contributorService.ApplyContributor(request);
            return new ApiResponse<ContributorResponse>(
                201,
                "Contributor application submitted successfully",
                result
            );
        }

        [HttpGet("my-application")]
        [Authorize(Roles = "MEMBER, CONTRIBUTOR, ADMIN")]
        public async Task<ApiResponse<ContributorApplyResponse?>> GetMyApplication()
        {
            var result = await contributorService.GetContributorApplication();
            return new ApiResponse<ContributorApplyResponse?>(
                code: 200,
                message: result == null ? "No application found" : "Application fetched successfully",
                result: result
            );
        }

        [HttpPut("{id}/reactivate")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<ContributorResponse>> ReActivate(int id)
        {
            var result = await contributorService.ReActivateContributor(id);
            return new ApiResponse<ContributorResponse>(
                200,
                "Contributor re-activated successfully",
                result
            );
        }

        [HttpGet("is_contributor_premium_eligible")]
        [Authorize(Roles = "CONTRIBUTOR")]
        public async Task<ApiResponse<bool>> IsContributorPremiumEligible()
        {
            return new ApiResponse<bool>(
                code: 200,
                message: "Check successfully",
                result: await contributorService.IsContributorPremiumEligible()
            );
        }
    }
}
