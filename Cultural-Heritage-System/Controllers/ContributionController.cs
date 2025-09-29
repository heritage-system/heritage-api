using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/contributions")]
    [ApiController]
    public class ContributionsController : ControllerBase
    {

        private readonly IContributionService contributionService;
        private readonly ITestSearchService testSearchService;

        public ContributionsController(IContributionService contributionService, ITestSearchService testSearchService)
        {
            this.contributionService = contributionService;
            this.testSearchService = testSearchService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<ContributionResponse>> CreateContribution([FromBody] ContributionCreationRequest request)
        {
            var users = await contributionService.PostContribution(request);

            return new ApiResponse<ContributionResponse>(
                code: 201,
                message: "Created contribution",
                result: users
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
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchName = null)
        {                  

            return new ApiResponse<PageResponse<ContributionSaveResponse>>(
                code: 200,
                message: "Get list contribution save successfully",
                result: await contributionService.GetContributionSave(page, pageSize, searchName)
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
    }
}
