using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
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
        public async Task<ContributionCreationResponse> PostContribution(ContributionCreationRequest request)
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

            return mapper.Map<ContributionCreationResponse>(contribution);
        }
    }
}
