using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("contributionDetail")]
        public async Task<ApiResponse<ContributionResponse>> GetHeritageDetail(int id)
        {
            var result = await contributionService.GetContributionDetail(id);
            return new ApiResponse<ContributionResponse>(
                code: 200,
                message: "Get contribution details successfully",
                result: result
            );
        }
    }
}
