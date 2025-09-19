using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IContributionReviewService
    {
        Task<ContributionReviewResponse> CreateReview(ContributionReviewCreateRequest request);
        Task<List<ContributionReviewResponse>> GetReviewsByContributionId(long contributionId);
        Task<LikeReviewResponse> ToggleLikeAsync(LikeReviewRequest request);
        Task<ReviewUpdateResponse> UpdateReview(ContributionReviewUpdateRequest request);
        Task<ReviewDeleteResponse> DeleteReview(long reviewId);
    }
}
