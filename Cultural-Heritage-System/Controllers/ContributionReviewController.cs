using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/contribution_reviews")]
    [ApiController]
    public class ContributionReviewController : ControllerBase
    {
        private readonly IContributionReviewService reviewService;
        private readonly ILogger<ContributionReviewController> logger;
        public ContributionReviewController(IContributionReviewService reviewService, ILogger<ContributionReviewController> logger)
        {
            this.reviewService = reviewService;
            this.logger = logger;
        }

        /// <summary>
        /// Create a new review (with optional media and replies).
        /// </summary>
        [HttpPost("create_review")]
        public async Task<ApiResponse<ContributionReviewResponse>> CreateContributionReview([FromForm] ContributionReviewCreateRequest request)
        {

            try
            {
                var review = await reviewService.CreateReview(request);
                return new ApiResponse<ContributionReviewResponse>(200, "Contribution Review created successfully", review);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateContributionReview");
                return new ApiResponse<ContributionReviewResponse>(500, ex.Message);
            }

        }

        /// <summary>
        /// Get all reviews by Heritage ID (with replies, likes, media).
        /// </summary>
        [HttpGet("get_contribution_reviews")]
        [AllowAnonymous] // Anyone can view reviews
        public async Task<ApiResponse<List<ContributionReviewResponse>>> GetContributionReviewsByHeritageId([FromQuery] int contributionId)
        {
            var reviews = await reviewService.GetReviewsByContributionId(contributionId);

            return new ApiResponse<List<ContributionReviewResponse>>(
                code: 200,
                message: "ContributionReviews retrieved successfully",
                result: reviews
            );
        }
        [HttpPost("like")]
        [Authorize] // User must be logged in
        public async Task<ApiResponse<LikeReviewResponse>> ToggleLike([FromBody] LikeReviewRequest request)
        {
            // Toggle like/unlike
            var response = await reviewService.ToggleLikeAsync(request);

            return new ApiResponse<LikeReviewResponse>(
                code: 200,
                message: request.Like ? "Contribution Review liked" : "Contribution Review unliked",
                result: response
            );
        }
        
        //[HttpPut]
        //public async Task<ApiResponse<ContributionReviewUpdateResponse>> UpdateContributionReview([FromForm] ContributionReviewUpdateRequest request)
        //{
        //    // Toggle like/unlike
        //    var response = await reviewService.UpdateContributionReview(request);

        //    return new ApiResponse<ContributionReviewUpdateResponse>(
        //        code: 200,
        //message: "update ContributionReviews",
        //        result: response
        //    );
        //}
        //[HttpDelete]
        //public async Task<ApiResponse<ContributionReviewDeleteResponse>> DeleteContributionReview([FromBody] ContributionReviewDeleteRequest request)
        //{
        //    // Toggle like/unlike
        //    var response = await reviewService.DeleteContributionReview(request);

        //    return new ApiResponse<ContributionReviewDeleteResponse>(
        //        code: 200,
        //message: response.message
        //    );
        //}

    }
}
