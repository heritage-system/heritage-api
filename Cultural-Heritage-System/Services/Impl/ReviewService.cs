using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ReviewService : IReviewService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ReviewRepository reviewRepository;
        private readonly UserRepository userRepository;
        private readonly IMapper mapper;
        private readonly ICloudinaryService cloudinaryService;
        private readonly ILogger<ReviewService> logger;

        public ReviewService(ReviewRepository reviewRepository, UserRepository userRepository, ILogger<ReviewService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, ICloudinaryService cloudinaryService)
        {
            this.reviewRepository = reviewRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task<ReviewResponse> CreateReview(ReviewCreateRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var review = mapper.Map<Review>(request);
            review.CreatedBy = accountIdClaim;
            review.UserId = int.Parse(accountIdClaim);

            // luôn khởi tạo list trống
            review.ReviewMedias = new List<ReviewMedia>();

            if (request.Media?.Any() == true)
            {
                foreach (var media in request.Media)
                {
                    if (media.File == null)
                        throw new ArgumentException("File is required for media upload.");

                    using var stream = media.File.OpenReadStream();

                    var uploadedUrl = media.Type switch
                    {
                        MediaType.IMAGE => await cloudinaryService.UploadImageAsync(stream, media.File.FileName),
                        MediaType.VIDEO => await cloudinaryService.UploadVideoAsync(stream, media.File.FileName),
                        MediaType.DOCUMENT => await cloudinaryService.UploadDocumentAsync(stream, media.File.FileName),
                        _ => throw new ArgumentException("Invalid media type")
                    };

                    review.ReviewMedias.Add(new ReviewMedia
                    {
                        Url = uploadedUrl,
                        MediaType = media.Type
                    });
                }
            }


            // 3. Save review
            await reviewRepository.AddAsync(review);

            // 4. Reload entity with navigation props => change to Repository
            var createdReview = await reviewRepository.GetReviewsWithIncludes()
     .FirstAsync(r => r.Id == review.Id);
            // 5. Map entity -> response
            return mapper.Map<ReviewResponse>(createdReview);
        }

        public async Task<List<ReviewResponse>> GetReviewsByHeritageId(long heritageId)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            int? currentUserId = string.IsNullOrEmpty(accountIdClaim) ? null : int.Parse(accountIdClaim);

            var reviews = await reviewRepository.GetReviewsWithIncludes()
      .Where(r => r.HeritageId == heritageId && r.ParentReviewId == null)
      .ToListAsync();

            // Map manually to set Likes count and LikedByMe
            return reviews.Select(r => MapReviewResponse(r, currentUserId)).ToList();
        }

        // Recursive mapper
        private ReviewResponse MapReviewResponse(Review review, int? currentUserId)
        {
            var response = mapper.Map<ReviewResponse>(review);

            // Set the actual review author's user ID
            response.UserId = review.UserId; // ✅ use review.UserId, not currentUserId

            // LikedByMe is still based on current user
            response.LikedByMe = currentUserId.HasValue &&
                                 review.Likes?.Any(l => l.UserId == currentUserId.Value) == true;
            response.CreatedByMe = currentUserId.HasValue &&
                                    int.Parse(review.CreatedBy) == currentUserId;
            // Recursively map replies
            if (review.Replies != null && response.Replies != null)
            {
                for (int i = 0; i < review.Replies.Count; i++)
                {
                    response.Replies[i] = MapReviewResponse(review.Replies.ElementAt(i), currentUserId);
                }
            }

            return response;
        }




        public async Task<LikeReviewResponse> ToggleLikeAsync(LikeReviewRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            int currentUserId = int.Parse(accountIdClaim);

            // Load the review including likes
            var review = await reviewRepository.GetReviewsQueryable()
                .Include(r => r.Likes)
                .FirstOrDefaultAsync(r => r.Id == request.ReviewId);

            if (review == null)
                throw new KeyNotFoundException("Review not found");

            // Check if user already liked
            var existingLike = review.Likes.FirstOrDefault(l => l.UserId == currentUserId);

            if (request.Like)
            {
                if (existingLike == null)
                {
                    review.Likes ??= new List<ReviewLike>();
                    review.Likes.Add(new ReviewLike
                    {
                        ReviewId = review.Id,
                        UserId = currentUserId
                    });
                }
            }
            else
            {
                if (existingLike != null)
                {
                    review.Likes.Remove(existingLike);
                }
            }


            // Save changes
            await reviewRepository.SaveChangesAsync();

            // Map using AutoMapper
            var response = mapper.Map<LikeReviewResponse>(review);

            // Set LikedByMe manually
            response.LikedByMe = review.Likes.Any(l => l.UserId == currentUserId);
            return response;
        }

        public IQueryable<Review> GetReviewsQueryable()
        {
            return reviewRepository.GetReviewsQueryable();
        }

        public async Task<ReviewUpdateResponse> UpdateReview(ReviewUpdateRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var review = await reviewRepository.GetReviewsQueryable()
                .Include(r => r.ReviewMedias)
                .FirstOrDefaultAsync(r => r.Id == request.Id);

            if (review == null)
                throw new KeyNotFoundException("Review not found");

            if (review.UserId != int.Parse(accountIdClaim))
                throw new AppException(ErrorCode.FORBIDDEN);

            // Update basic properties via AutoMapper
            mapper.Map(request, review); // Maps Comment and other simple fields

            // Handle media update if any
            if (request.Media != null && request.Media.Any())
            {
                review.ReviewMedias.Clear(); // remove old media

                foreach (var media in request.Media)
                {
                    if (media.File == null)
                        throw new ArgumentException("File is required for media upload.");

                    using var stream = media.File.OpenReadStream();

                    // media.Type is already MediaType enum
                    var uploadedUrl = media.Type switch
                    {
                        MediaType.IMAGE => await cloudinaryService.UploadImageAsync(stream, media.File.FileName),
                        MediaType.VIDEO => await cloudinaryService.UploadVideoAsync(stream, media.File.FileName),
                        MediaType.DOCUMENT => await cloudinaryService.UploadDocumentAsync(stream, media.File.FileName),
                        _ => throw new ArgumentException("Invalid media type")
                    };

                    review.ReviewMedias.Add(new ReviewMedia
                    {
                        Url = uploadedUrl,
                        MediaType = media.Type
                    });
                }
            }


            review.UpdatedAt = DateTime.UtcNow;

            await reviewRepository.SaveChangesAsync();

            // Map to response DTO
            var response = mapper.Map<ReviewUpdateResponse>(review);
            return response;
        }

        public async Task<ReviewDeleteResponse> DeleteReview(ReviewDeleteRequest request)
        {
            // Get current user id from HttpContext
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            int currentUserId = int.Parse(accountIdClaim);

            // Find the review
            var review = await reviewRepository.GetReviewsQueryable()
                .SingleOrDefaultAsync(r => r.Id == request.id);

            if (review == null)
                return new ReviewDeleteResponse
                {
                    ReviewId = request.id,
                    success = false,
                    message = "Review not found"

                };

            // Optional: check ownership
            if (review.UserId != currentUserId)
                return new ReviewDeleteResponse
                {
                    ReviewId = request.id,
                    success = false,
                    message = "You are not allowed to delete this review"
                };

            // Remove review
            await reviewRepository.DeleteAsync(review);

            // Map to response DTO using AutoMapper
            var response = mapper.Map<ReviewDeleteResponse>(review);
            response.success = true;
            response.message = "Review deleted successfully";
            return response;
        }

    }
}
