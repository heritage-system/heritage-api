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
        private readonly ContributorRepository contributorRepository;
        private readonly ContributionRepository contributionRepository;
        private readonly SubscriptionRepository subscriptionRepository;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<ContributionService> logger;
        private readonly ContributionAccessLogRepository contributionAccessLogRepository;
        private readonly ContributionUnlockRepository contributionUnlockRepository;
        private readonly ContributionSaveRepository contributionSaveRepository;
        private readonly ContributionReviewRepository contributionReviewRepository;
        public ContributionReviewService(ContributorRepository contributorRepository, ContributionRepository contributionRepository, ILogger<ContributionService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, SubscriptionRepository subscriptionRepository, 
            ContributionAccessLogRepository contributionAccessLogRepository, ContributionUnlockRepository contributionUnlockRepository, 
            ContributionSaveRepository contributionSaveRepository, ContributionReviewRepository contributionReviewRepository)
        {
            this.contributorRepository = contributorRepository;
            this.logger = logger;
            this.contributionRepository = contributionRepository;
            this.mailService = mailService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;
            this.contributionAccessLogRepository = contributionAccessLogRepository;
            this.contributionUnlockRepository = contributionUnlockRepository;
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
            return mapper.Map<ContributionReviewResponse>(createdReview);
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
            throw new NotImplementedException();
        }

        public async Task<ReviewUpdateResponse> UpdateReview(ContributionReviewUpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<ReviewDeleteResponse> DeleteReview(long reviewId)
        {
            throw new NotImplementedException();
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
