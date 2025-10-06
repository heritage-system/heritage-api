using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.Packaging.Ionic.Zlib;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ContributionReviewService : IContributionReviewService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IContributorRepository contributorRepository;
        private readonly IContributionRepository contributionRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<ContributionService> logger;
        private readonly IContributionAccessLogRepository contributionAccessLogRepository;        
        private readonly IContributionSaveRepository contributionSaveRepository;
        private readonly IContributionReviewRepository contributionReviewRepository;
        public ContributionReviewService(IContributorRepository contributorRepository, IContributionRepository contributionRepository, ILogger<ContributionService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository, 
            IContributionAccessLogRepository contributionAccessLogRepository, 
            IContributionSaveRepository contributionSaveRepository, IContributionReviewRepository contributionReviewRepository)
        {
            this.contributorRepository = contributorRepository;
            this.logger = logger;
            this.contributionRepository = contributionRepository;
            this.mailService = mailService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;
            this.contributionAccessLogRepository = contributionAccessLogRepository;
            this.contributionSaveRepository = contributionSaveRepository;
            this.contributionReviewRepository = contributionReviewRepository;
        }
       
        public async Task<ContributionReviewResponse> CreateReview(ContributionReviewCreateRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var review = mapper.Map<ContributionReview>(request);
         
            review.UserId = int.Parse(accountIdClaim);

          
            await contributionReviewRepository.AddAsync(review);        
            var createdReview = await contributionReviewRepository.GetContributionReviewById(review.Id);         
            var result =  mapper.Map<ContributionReviewResponse>(createdReview);
            result.CreatedByMe = true;
            return result;
        }

        public async Task<List<ContributionReviewResponse>> GetReviewsByContributionId(long contributionId)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            int? currentUserId = string.IsNullOrEmpty(accountIdClaim) ? null : int.Parse(accountIdClaim);
           
            var reviews = await contributionReviewRepository.GetContributionReviewReviewsHierarchy(contributionId);

            return reviews.Select(r => MapReviewResponse(r, currentUserId)).ToList();
        }

        public async Task<LikeReviewResponse> ToggleLikeAsync(LikeReviewRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            int currentUserId = int.Parse(accountIdClaim);

            // Load the review including likes
            var review = await contributionReviewRepository.GetContributionReviewById(request.ReviewId);               

            if (review == null)
                throw new AppException(ErrorCode.REVIEW_NOT_FOUND);

            // Check if user already liked
            var existingLike = review.Likes.FirstOrDefault(l => l.UserId == currentUserId);

            if (request.Like)
            {
                if (existingLike == null)
                {
                    review.Likes ??= new List<ContributionReviewLike>();
                    review.Likes.Add(new ContributionReviewLike
                    {
                        ContributionReviewId = review.Id,
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
            await contributionReviewRepository.SaveChangesAsync();

            // Map using AutoMapper
            var response = mapper.Map<LikeReviewResponse>(review);

            // Set LikedByMe manually
            response.LikedByMe = review.Likes.Any(l => l.UserId == currentUserId);
            return response;
        }

        public async Task<ContributionReviewUpdateResponse> UpdateReview(ContributionReviewUpdateRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var review = await contributionReviewRepository.GetContributionReviewById(request.Id);

            if (review == null)
                throw new AppException(ErrorCode.REVIEW_NOT_FOUND);

            if (review.UserId != int.Parse(accountIdClaim))
                throw new AppException(ErrorCode.FORBIDDEN);

           
            mapper.Map(request, review);
        
            review.UpdatedAt =DateTime.Now;
            await contributionReviewRepository.UpdateAsync(review);

            // Map to response DTO
            var response = mapper.Map<ContributionReviewUpdateResponse>(review);
            return response;
        }

        public async Task<bool> DeleteReview(long reviewId)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            int currentUserId = int.Parse(accountIdClaim);

            // Find the review
            var review = await contributionReviewRepository.GetContributionReviewById(reviewId);

            if (review == null)
                throw new AppException(ErrorCode.REVIEW_NOT_FOUND);

            // Optional: check ownership
            if (review.UserId != currentUserId)
                throw new AppException(ErrorCode.UNAUTHORIZED);

            // Remove review
            await contributionReviewRepository.DeleteContributionReviewWithRepliesAsync(review.Id);
        
            return true;
        }

        private ContributionReviewResponse MapReviewResponse(ContributionReview review, int? currentUserId)
        {
            if (review == null) return null;

            var response = mapper.Map<ContributionReviewResponse>(review);

           
            response.UserId = review.UserId;
            response.LikedByMe = currentUserId.HasValue &&
                                 review.Likes?.Any(l => l.UserId == currentUserId.Value) == true;
            response.CreatedByMe = currentUserId.HasValue &&
                                   review.UserId == currentUserId.Value;

           
            response.Replies = review.Replies?
                .Select(r => MapReviewResponse(r, currentUserId))
                .ToList() ?? new List<ContributionReviewResponse>();

            return response;
        }

    }
}
