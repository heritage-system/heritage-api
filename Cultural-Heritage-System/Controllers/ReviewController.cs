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
    [Route("api/v1/Reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;
        private readonly ILogger<ReviewController> logger;
        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            this.reviewService = reviewService;
            this.logger = logger;
        }

        /// <summary>
        /// Create a new review (with optional media and replies).
        /// </summary>
        [HttpPost("create_review")]
        public async Task<ApiResponse<ReviewResponse>> CreateReview([FromBody] ReviewCreateRequest request)
        {

            try
            {
                var review = await reviewService.CreateReview(request);
                return new ApiResponse<ReviewResponse>(200, "Review created successfully", review);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateReview");
                return new ApiResponse<ReviewResponse>(500, ex.Message);
            }

        }

        /// <summary>
        /// Get all reviews by Heritage ID (with replies, likes, media).
        /// </summary>
        [HttpGet("reviewByheritage")]
        
        public async Task<ApiResponse<List<ReviewResponse>>> GetReviewsByHeritageId([FromQuery] long heritageId)
        {
            var reviews = await reviewService.GetReviewsByHeritageId(heritageId);

            return new ApiResponse<List<ReviewResponse>>(
                code: 200,
                message: "Reviews retrieved successfully",
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
                message: request.Like ? "Review liked" : "Review unliked",
                result: response
            );
        }
        [HttpGet]
        public async Task<ApiResponse<IQueryable<Review>>> GetReviews()
        {

            var response = reviewService.GetReviewsQueryable();

            return new ApiResponse<IQueryable<Review>>(
                code: 200,
        message: "get Reviews",
                result: response
            );
        }
        [HttpPut]
        public async Task<ApiResponse<ReviewUpdateResponse>> UpdateReview([FromForm] ReviewUpdateRequest request)
        {
            // Toggle like/unlike
            var response = await reviewService.UpdateReview(request);

            return new ApiResponse<ReviewUpdateResponse>(
                code: 200,
        message: "update Reviews",
                result: response
            );
        }
        [HttpDelete]
        public async Task<ApiResponse<ReviewDeleteResponse>> DeleteReview([FromBody] ReviewDeleteRequest request)
        {
            // Toggle like/unlike
            var response = await reviewService.DeleteReview(request);

            return new ApiResponse<ReviewDeleteResponse>(
                code: 200,
        message: response.message
            );
        }

    }
}
