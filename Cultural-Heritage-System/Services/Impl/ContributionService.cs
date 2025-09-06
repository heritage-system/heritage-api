using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ContributionService : IContributionService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ContributorRepository contributorRepository;
        private readonly ContributionRepository contributionRepository;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<ContributionService> logger;

        public ContributionService(ContributorRepository contributorRepository, ContributionRepository contributionRepository, ILogger<ContributionService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.contributorRepository = contributorRepository;
            this.logger = logger;
            this.contributionRepository = contributionRepository;
            this.mailService = mailService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task<PageResponse<ContributionSearchResponse>> SearchContributionsAsync(ContributionSearchRequest request)
        {
            try
            {
                var query = contributionRepository.GetContributionsQueryable();

               
                // Keyword search
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var searchTerm = request.Keyword.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                    query = query.Where(h =>
                        h.Title.ToLower().Contains(searchTerm) ||
                        h.Contributor.User.UserName.ToLower().Contains(searchTerm) ||


                        h.TitleUnsigned.Contains(unsignedTerm) ||
                        h.Contributor.User.UserNameUnsigned.Contains(unsignedTerm));
                }            

                //// Category filter
                //if (request.CategoryIds != null && request.CategoryIds.Any())
                //{
                //    query = query.Where(h => request.CategoryIds.Contains(h.CategoryId));
                //}

                //// Tag filter
                //if (request.TagIds != null && request.TagIds.Any())
                //{
                //    query = query.Where(h => h.HeritageTags.Any(t => request.TagIds.Contains(t.TagId)));
                //}
              
                switch (request.SortBy)
                {
                    case SortBy.IDASC:
                        query = query.OrderBy(h => h.Id);
                        break;
                    case SortBy.IDDESC:
                        query = query.OrderByDescending(h => h.Id);
                        break;
                    case SortBy.NAMEASC:
                        query = query.OrderBy(h => h.Title);
                        break;
                    case SortBy.NAMEDESC:
                        query = query.OrderByDescending(h => h.Title);
                        break;
                    default:
                        query = query.OrderBy(h => h.Id);
                        break;
                }

                var dtoQuery = query.ProjectTo<ContributionSearchResponse>(mapper.ConfigurationProvider);
                // Pagination
                var response = dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching heritages");
                throw;
            }
        }

        public async Task<ContributionResponse> PostContribution(ContributionCreationRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;       
            Contribution contribution = mapper.Map<Contribution>(request);

            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            var currentContributor = await contributorRepository.GetContributorByUserId(int.Parse(accountIdClaim));
            if (currentContributor == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            contribution.ContributorId = currentContributor.Id;

            await contributionRepository.AddAsync(contribution);

            return mapper.Map<ContributionResponse>(contribution);
        }

        public async Task<ContributionResponse> GetContributionDetail(int id)
        {
            var existingContribution = await contributionRepository.GetContributionById(id);

            if (existingContribution == null)
            {
                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);
            }

            var response = mapper.Map<ContributionResponse>(existingContribution);

            return response;
        }
    }
}
