using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Contribution;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/contribution-acceptances")]
    [ApiController]
    public class ContributionAcceptanceController : ControllerBase
    {
        private readonly IContributionAcceptanceService _service;

        public ContributionAcceptanceController(IContributionAcceptanceService service)
        {
            _service = service;
        }

        [HttpPost("staff/acceptances/{id}/approve")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<ApiResponse<object>> Approve(long id)
        {
            await _service.ApproveOrRejectAsync(id, ContributionStatus.APPROVED);
            return new ApiResponse<object>(
                200,
                "Contribution approved successfully",
                new { acceptanceId = id, status = ContributionStatus.APPROVED }
            );
        }

        [HttpPost("staff/acceptances/{id}/reject")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<ApiResponse<object>> Reject(long id, [FromBody] ContributionAcceptanceDecisionRequest request)
        {
            await _service.ApproveOrRejectAsync(id, ContributionStatus.REJECTED, request.Note);
            return new ApiResponse<object>(
                200,
                "Contribution rejected successfully",
                new { acceptanceId = id, status = ContributionStatus.REJECTED }
            );
        }

        [HttpGet("staff/contributions/{contributionId}/overview")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<ApiResponse<ContributionOverviewResponse>> GetContributionOverviewForStaff(
            int contributionId)
        {
            var result = await _service.GetContributionOverviewForStaff(contributionId);
            return new ApiResponse<ContributionOverviewResponse>(
                code: 200,
                message: "Contribution overview fetched successfully",
                result: result
            );
        }

        [HttpGet("staff/contributions-overview")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<ApiResponse<PageResponse<ContributionOverviewListItemResponse>>>
            GetListContributionsOverviewForStaff([FromQuery] ContributionOverviewSearchRequest request)
        {
            var result = await _service.GetListContributionsOverviewForStaff(request);
            return new ApiResponse<PageResponse<ContributionOverviewListItemResponse>>(
                code: 200,
                message: "Contributions overview list fetched successfully",
                result: result
            );
        }
    }
}
