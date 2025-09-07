using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IReviewService
    {
        Task<ReviewResponse> CreateReview(ReviewCreateRequest request);
        Task<List<ReviewResponse>> GetReviewsByHeritageId(long heritageId);
        Task<LikeReviewResponse> ToggleLikeAsync(LikeReviewRequest request);
        Task<ReviewUpdateResponse> UpdateReview(ReviewUpdateRequest request);
        Task<ReviewDeleteResponse> DeleteReview(ReviewDeleteRequest request);
        IQueryable<Review> GetReviewsQueryable();
    }
}
