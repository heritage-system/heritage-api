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
        private readonly UserRepository userRepository;
        private readonly ContributionRepository contributionRepository;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<ContributionService> logger;

        public ContributionService(UserRepository userRepository, ContributionRepository contributionRepository, ILogger<ContributionService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
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

            //Un-contributor id
            contribution.ContributorId = 1;
         
            await contributionRepository.AddAsync(contribution);

            return mapper.Map<ContributionCreationResponse>(contribution);
        }
    }
}
