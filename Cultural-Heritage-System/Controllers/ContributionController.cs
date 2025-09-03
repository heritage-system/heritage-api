using Cultural_Heritage_System.Dtos.Request;
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
        public async Task<ApiResponse<ContributionCreationResponse>> CreateContribution([FromBody] ContributionCreationRequest request)
        {
            var users = await contributionService.PostContribution(request);

            return new ApiResponse<ContributionCreationResponse>(
                code: 201,
                message: "Created contribution",
                result: users
            );
        }     
    }
}
