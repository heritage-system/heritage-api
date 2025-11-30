using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/contributions")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {

        private readonly IContributionService contributionService;


        public ContributionsController(IContributionService contributionService)
        {
            this.contributionService = contributionService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<ContributionResponse>> CreateContribution([FromBody] ContributionCreationRequest request)
        {

            return new ApiResponse<ContributionResponse>(
                code: 201,
                message: "Created contribution successfully",
                result: await contributionService.PostContribution(request)
            );
        }

        [HttpGet("search_contribution")]
        public async Task<ApiResponse<PageResponse<ContributionSearchResponse>>> GetAllWithSearch(
           [FromQuery] ContributionSearchRequest request)

        {
            return new ApiResponse<PageResponse<ContributionSearchResponse>>
            {
                code = 200,
                result = await contributionService.SearchContributionsAsync(request)
            };
        }

        [HttpGet("get_contribution_detail")]
        public async Task<ApiResponse<ContributionResponse>> GetContributionDetail(int id)
        {
            var result = await contributionService.GetContributionDetail(id);
            return new ApiResponse<ContributionResponse>(
                code: 200,
                message: "Get contribution details successfully",
                result: result
            );
        }

        //[HttpPost("unlock_contribution")]
        //public async Task<ApiResponse<ContributionResponse>> UnlockContribution(int id)
        //{          
        //    return new ApiResponse<ContributionResponse>(
        //        code: 200,
        //        message: "Unlock contribution successfully",
        //        result: await contributionService.UnlockContribution(id)
        //    );
        //}

        [HttpGet("get_contribution_save")]
        public async Task<ApiResponse<PageResponse<ContributionSaveResponse>>> GetContributionSaves(
            [FromQuery] ContributionSearchRequest request)
        {

            return new ApiResponse<PageResponse<ContributionSaveResponse>>(
                code: 200,
                message: "Get list contribution save successfully",
                result: await contributionService.GetContributionSave(request)
            );

        }

        [HttpPost("add_contribution_save")]
        public async Task<ApiResponse<bool>> AddContributionSave(int id)
        {
            return new ApiResponse<bool>(
                code: 201,
                message: "Added to contribution save successfully",
                result: await contributionService.AddContributionSave(id)
            );

        }

        [HttpDelete("remove_contribution_save")]
        public async Task<ApiResponse<object>> RemoveContributionSave(int id)
        {

            return new ApiResponse<object>(
                code: 200,
                message: "Removed from contribution save successfully",
                result: await contributionService.RemoveContributionSave(id)
            );

        }

        [HttpGet("top_contribution_heritage_tag")]
        public async Task<ApiResponse<List<TopContributionHeritageTagResponse>>> GetTrendingContributionHeritageTag()
        {

            return new ApiResponse<List<TopContributionHeritageTagResponse>>(
                code: 200,
                message: "Get list top contribution heritage tag successfully",
                result: await contributionService.GetTrendingContributionHeritageTag()
            );

        }

        [HttpGet("top_contributor")]
        public async Task<ApiResponse<List<TrendingContributorDto>>> GetTrendingContributor()
        {

            return new ApiResponse<List<TrendingContributorDto>>(
                code: 200,
                message: "Get list top contributor successfully",
                result: await contributionService.GetTrendingContributor()
            );

        }

        [HttpPost("create_contribution_report")]
        public async Task<ApiResponse<bool>> CreateContributionReport(ContributionReportCreationRequest request)
        {
            return new ApiResponse<bool>(
                code: 201,
                message: "Create contribution report successfully",
                result: await contributionService.CreateContributionReport(request)
            );

        }

        [HttpGet("contribution_related")]
        public async Task<ApiResponse<List<ContributionSearchResponse>>> GetContributionRelated(
          [FromQuery] ContributionRelatedRequest request)

        {
            return new ApiResponse<List<ContributionSearchResponse>>
            {
                code = 200,
                result = await contributionService.GetContributionRelated(request)
            };
        }

        [HttpGet("get_contribution_overview")]
        public async Task<ApiResponse<ContributionOverviewResponse>> GetContributionOverview(int id)
        {
            var result = await contributionService.GetContributionOverview(id);
            return new ApiResponse<ContributionOverviewResponse>(
                code: 200,
                message: "Get contribution overview successfully",
                result: result
            );
        }

        [HttpGet("get_list_contribution_overview")]
        public async Task<ApiResponse<PageResponse<ContributionOverviewListItemResponse>>> GetListContributionsOverview(
           [FromQuery] ContributionOverviewSearchRequest request)

        {
            return new ApiResponse<PageResponse<ContributionOverviewListItemResponse>>
            {
                code = 200,
                result = await contributionService.GetListContributionsOverview(request)
            };
        }

        [HttpGet("get_contribution_updated")]
        public async Task<ApiResponse<ContributionDetailUpdatedResponse>> GetContributionDetailUpdated(int id)
        {
            var result = await contributionService.GetContributionDetailForUpdated(id);
            return new ApiResponse<ContributionDetailUpdatedResponse>(
                code: 200,
                message: "Get contribution detail successfully",
                result: result
            );
        }

        [HttpPut("updated_contribution")]
        [AllowAnonymous]
        public async Task<ApiResponse<ContributionResponse>> UpdateContribution([FromBody] ContributionUpdateRequest request)
        {
            var users = await contributionService.UpdateContribution(request);

            return new ApiResponse<ContributionResponse>(
                code: 200,
                message: "Updated contribution successfully",
                result: users
            );
        }
    }
}
