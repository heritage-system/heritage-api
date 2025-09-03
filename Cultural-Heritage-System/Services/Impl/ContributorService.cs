using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ContributorService : IContributorService
    {
        private readonly ContributorRepository contributorRepository;
        private readonly ILogger<ContributorService> logger;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ContributorService(
            ContributorRepository contributorRepository,
            ILogger<ContributorService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            this.contributorRepository = contributorRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        //public async Task<PageResponse<ContributorResponse>> SearchContributorsAsync(ContributorSearchRequest request)
        //{
        //    var query = contributorRepository.GetContributorsQueryable();

        //    if (!string.IsNullOrEmpty(request.Keyword))
        //    {
        //        var searchTerm = request.Keyword.Trim().ToLower();
        //        var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

        //        query = query.Where(h =>
        //            h.User.FullName.ToLower().Contains(searchTerm) ||
        //            h.Expertise.ToLower().Contains(searchTerm) ||
        //            h.ExpertiseUnsigned.Contains(unsignedTerm) ||
        //            h.User.FullNameUnsigned.Contains(unsignedTerm));
        //    }

        //    if (request.Verified.HasValue)
        //    {
        //        query = query.Where(c => c.Verified == request.Verified.Value);
        //    }

        //    if (request.Status.HasValue)
        //    {
        //        query = query.Where(c => c.Status == request.Status.Value);
        //    }

        //    query = request.SortBy switch
        //    {
        //        SortBy.IDASC => query.OrderBy(c => c.Id),
        //        SortBy.IDDESC => query.OrderByDescending(c => c.Id),
        //        SortBy.NAMEASC => query.OrderBy(c => c.User.FullName),
        //        SortBy.NAMEDESC => query.OrderByDescending(c => c.User.FullName),
        //        _ => query.OrderBy(c => c.Id)
        //    };

        //    var dtoQuery = query.ProjectTo<ContributorResponse>(mapper.ConfigurationProvider);
        //    return dtoQuery.ToPagedResponse(request.Page, request.PageSize);
        //}

        public async Task<ContributorResponse> GetContributorDetail(int id)
        {
            var contributor = await contributorRepository.GetContributorById(id);
            if (contributor == null)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
            }

            return mapper.Map<ContributorResponse>(contributor);
        }

        public async Task<ContributorResponse> CreateContributor(ContributorCreateRequest request)
        {      
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var userId = int.Parse(accountIdClaim);

            var contributor = mapper.Map<Contributor>(request);
            contributor.UserId = userId; 

            await contributorRepository.AddAsync(contributor);
            return mapper.Map<ContributorResponse>(contributor);
        }

        public async Task<ContributorResponse> UpdateContributor(int id, ContributorUpdateRequest request)
        {
            var contributor = await contributorRepository.GetContributorById(id);
            if (contributor == null)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
            }

            mapper.Map(request, contributor);
            await contributorRepository.UpdateAsync(contributor);
            return mapper.Map<ContributorResponse>(contributor);
        }

        public async Task DeleteContributor(int id)
        {
            var contributor = await contributorRepository.GetContributorById(id);
            if (contributor == null)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
            }

            await contributorRepository.DeleteAsync(contributor);
        }
    }
}
