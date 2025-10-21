using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ContributionService : IContributionService
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
        private readonly IContributionHeritageTagRepository contributionHeritageTagRepository;
        private readonly IContributionReportRepository contributionReportRepository;
        private readonly IStaffRepository staffRepository;
        public ContributionService(IContributorRepository contributorRepository, IContributionRepository contributionRepository, ILogger<ContributionService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository,
            IContributionAccessLogRepository contributionAccessLogRepository,
            IContributionSaveRepository contributionSaveRepository, IContributionHeritageTagRepository contributionHeritageTagRepository, IContributionReportRepository contributionReportRepository, IStaffRepository staffRepository)
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
            this.contributionHeritageTagRepository = contributionHeritageTagRepository;
            this.contributionReportRepository = contributionReportRepository;
            this.staffRepository = staffRepository;
        }

        public async Task<PageResponse<ContributionSearchResponse>> SearchContributionsAsync(ContributionSearchRequest request)
        {
            try
            {
                var query = contributionRepository.GetApprovedContributionsQueryable();


                // Keyword search
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var searchTerm = request.Keyword.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                    query = query.Where(h =>
                        // Contribution title
                        h.Title.ToLower().Contains(searchTerm) ||
                        h.TitleUnsigned.Contains(unsignedTerm) ||

                        // Contributor username
                        h.Contributor.User.UserName.ToLower().Contains(searchTerm) ||
                        h.Contributor.User.UserNameUnsigned.Contains(unsignedTerm) ||

                        // Heritage liên quan qua ContributionHeritageTag
                        h.ContributionHeritageTags.Any(tag =>
                            tag.Heritage.Name.ToLower().Contains(searchTerm) ||
                            tag.Heritage.NameUnsigned.Contains(unsignedTerm)
                        )
                    );
                }


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
                        query = query.OrderByDescending(h => h.CreatedAt);
                        break;
                }


                var dtoQuery = query.ProjectTo<ContributionSearchResponse>(mapper.ConfigurationProvider);

                // Pagination
                var response = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);

                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(accountIdClaim) && response.Items != null)
                {
                    var userId = int.Parse(accountIdClaim);

                    foreach (var item in response.Items)
                    {
                        item.IsSave = await contributionSaveRepository.IsContributionSaveExists(userId, item.Id);
                    }
                }


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
            if (!currentContributor.IsPremiumEligible && request.PremiumType == PremiumType.SUBSCRIPTIONONLY)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_IS_NOT_PREMIUM_ELIGIBLE);
            }

            if (request.TagHeritageIds?.Any() == true)
            {
                contribution.ContributionHeritageTags = request.TagHeritageIds.Select(o => new ContributionHeritageTag
                {
                    HeritageId = o
                }).ToList();
            }
            contribution.ContributorId = currentContributor.Id;

            contribution.PreviewContent = DeltaHelper.GeneratePreviewDelta(contribution.Content);

            contribution.FirstContent = DeltaHelper.ExtractFirstLongParagraph(contribution.Content);

            var nextStaffId = await GetNextStaffForContributionAsync();
            if (nextStaffId != null)
            {
                contribution.ContributionAcceptances.Add(new ContributionAcceptance
                {
                    StaffId = nextStaffId.Value,                 
                    Note = "Bài viết mới, chờ duyệt"
                });
            }


            await contributionRepository.AddAsync(contribution);

            return mapper.Map<ContributionResponse>(contribution);
        }

        public async Task<ContributionResponse> GetContributionDetail(int id)
        {
            var existingContribution = await contributionRepository.GetContributionByIdAndStatus(id, ContributionStatus.APPROVED);

            if (existingContribution == null)
                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);

            var response = mapper.Map<ContributionResponse>(existingContribution);

            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            // Nếu free thì trả luôn          
            if (existingContribution.PremiumType == PremiumType.FREE)
            {

                if (!string.IsNullOrEmpty(accountIdClaim))
                {
                    var userFreeId = int.Parse(accountIdClaim);

                    response.IsSave = await contributionSaveRepository.IsContributionSaveExists(userFreeId, id);
                    // kiểm tra đã có log chưa
                    var existingFreeLog = await contributionAccessLogRepository.GetContributionAccessLogs(userFreeId, existingContribution.Id);

                    if (existingFreeLog == null)
                    {
                        var log = new ContributionAccessLog
                        {
                            UserId = userFreeId,
                            ContributionId = existingContribution.Id,
                        };

                        await contributionAccessLogRepository.AddAsync(log);
                    }
                    else
                    {
                        existingFreeLog.UpdatedAt = DateTime.UtcNow;
                        await contributionAccessLogRepository.UpdateAsync(existingFreeLog);
                    }
                }

                return response;
            }


            // Premium → check used        
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                // chưa login → chỉ preview
                response.Content = null;
                return response;
            }

            var userId = int.Parse(accountIdClaim);

            response.IsSave = await contributionSaveRepository.IsContributionSaveExists(userId, id);

            // Lấy subscription active
            var activeSub = await subscriptionRepository.GetActiveSubscription(userId);

            if (activeSub == null)
            {
                // không có sub → chỉ preview
                response.Content = null;
                return response;
            }


            response.Subscription = mapper.Map<SubscriptionDto>(activeSub);

            //var unlockContribution = await contributionUnlockRepository.GetContributionUnlocks(userId, id);

            //if (unlockContribution == null)
            //{
            //    // không có sub → chỉ preview
            //    response.Content = null;
            //    return response;
            //}



            return response;
        }

        //public async Task<ContributionResponse> UnlockContribution(int contributionId)
        //{
        //    var existingContribution = await contributionRepository.GetContributionById(contributionId);

        //    if (existingContribution == null)
        //        throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);

        //    var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        //    if (string.IsNullOrEmpty(accountIdClaim))
        //    {
        //        throw new AppException(ErrorCode.UNAUTHORIZED);
        //    }

        //    var userId = int.Parse(accountIdClaim);

        //    // Lấy subscription active
        //    var activeSub = await subscriptionRepository.GetActiveSubscription(userId);

        //    if (activeSub == null)
        //    {
        //        throw new AppException(ErrorCode.USER_NOT_PREMIUM);
        //    }

        //    if (activeSub.OpensUsed >= activeSub.Package.MaxOpensPerMonth)
        //    {
        //        throw new AppException(ErrorCode.OVER_OPEN_LIMIT);
        //    }

        //    activeSub.OpensUsed++;
        //    await subscriptionRepository.UpdateAsync(activeSub);

        //    var unlock = new ContributionUnlock
        //    {
        //        UserId = userId,
        //        ContributionId = contributionId
        //    };
        //    await contributionUnlockRepository.AddAsync(unlock);

        //    var existingLog = await contributionAccessLogRepository.GetContributionAccessLogs(userId, existingContribution.Id);

        //    if (existingLog == null)
        //    {
        //        // Chưa mở bài này

        //        var log = new ContributionAccessLog
        //        {
        //            UserId = userId,
        //            ContributionId = existingContribution.Id,
        //            SubscriptionId = activeSub.Id,
        //            CountedForQuota = true
        //        };

        //        await contributionAccessLogRepository.AddAsync(log);

        //    }
        //    else
        //    {
        //        // Đã mở → update LastOpenedAt
        //        existingLog.UpdatedAt = DateTime.UtcNow;
        //        await contributionAccessLogRepository.UpdateAsync(existingLog);
        //    }

        //    var result = mapper.Map<ContributionResponse>(existingContribution);
        //    return result;
        //}

        public async Task<PageResponse<ContributionSaveResponse>> GetContributionSave(ContributionSearchRequest request)
        {
            try
            {
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (accountIdClaim == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }

                var userId = int.Parse(accountIdClaim);
                var query = contributionSaveRepository.GetContributionSavesQueryByUserId(userId);

                // Apply search filter if searchName is provided
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var searchTerm = request.Keyword.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                    query = query.Where(f =>
                        f.Contribution.Title.ToLower().Contains(searchTerm) ||
                        f.Contribution.TitleUnsigned.Contains(unsignedTerm));
                }

                var dtoQuery = query.ProjectTo<ContributionSaveResponse>(mapper.ConfigurationProvider);

                return await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting contribution save for current user");
                throw;
            }
        }

        public async Task<bool> AddContributionSave(int contributionId)
        {
            try
            {
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;


                if (accountIdClaim == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }


                var existingContribution = await contributionRepository.GetContributionByIdAndStatus(contributionId, ContributionStatus.APPROVED);
                if (existingContribution == null)
                {

                    throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);
                }

                var userId = int.Parse(accountIdClaim);

                var existingContributionSave = await contributionSaveRepository.GetContributionSaveByUserAndContribution(userId, contributionId);
                if (existingContributionSave != null)
                {
                    throw new AppException(ErrorCode.CONTRIBUTION_SAVE_ALREADY_EXISTS);
                }

                var save = new ContributionSave
                {
                    UserId = userId,
                    ContributionId = contributionId
                };

                await contributionSaveRepository.AddAsync(save);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> RemoveContributionSave(int contributionId)
        {
            try
            {
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;


                if (accountIdClaim == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }


                var existingContribution = await contributionRepository.GetContributionByIdAndStatus(contributionId,ContributionStatus.APPROVED);
                if (existingContribution == null)
                {

                    throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);
                }

                var userId = int.Parse(accountIdClaim);

                var existingContributionSave = await contributionSaveRepository.GetContributionSaveByUserAndContribution(userId, contributionId);
                if (existingContributionSave == null)
                {
                    throw new AppException(ErrorCode.CONTRIBUTION_SAVE_NOT_FOUND);
                }

                // Remove favorite
                await contributionSaveRepository.DeleteAsync(existingContributionSave);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TopContributionHeritageTagResponse>> GetTrendingContributionHeritageTag()
        {
            var topHeritages = await contributionHeritageTagRepository
                .GetContributionHeritageTagsQueryable()
                .Where(h => h.Contribution.Status == ContributionStatus.APPROVED)
                .GroupBy(x => new { x.HeritageId, x.Heritage.Name })
                .Select(g => new TopContributionHeritageTagResponse
                {
                    HeritageId = g.Key.HeritageId,
                    HeritageName = g.Key.Name,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            return topHeritages;

        }

        public async Task<List<TrendingContributorDto>> GetTrendingContributor()
        {
            return await contributionRepository.GetTopContributorsAsync();
        }

        public async Task<bool> CreateContributionReport(ContributionReportCreationRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;


            if (accountIdClaim == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var existingContribution = await contributionRepository.GetContributionByIdAndStatus(request.ContributionId,ContributionStatus.APPROVED);
            if (existingContribution == null)
            {

                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);
            }

            var contributionReport = mapper.Map<ContributionReport>(request);
            contributionReport.UserId = int.Parse(accountIdClaim);
            await contributionReportRepository.AddAsync(contributionReport);
            return true;
        }

        public async Task<List<ContributionSearchResponse>> GetContributionRelated(ContributionRelatedRequest request)
        {
            try
            {
                var initQuery = contributionRepository.GetApprovedContributionsQueryable();

                var query = initQuery;
                if (request.contributionId.HasValue)
                {
                    query = query.Where(c => c.Id != request.contributionId.Value);
                }

                // Ưu tiên theo TagHeritageIds
                if (request.TagHeritageIds != null && request.TagHeritageIds.Any())
                {
                    query = query.Where(c =>
                        c.ContributionHeritageTags.Any(tag => request.TagHeritageIds.Contains((int)tag.HeritageId)));
                }
                // Nếu không có tag thì ưu tiên ContributorIds
                else if (request.ContributorIds != null && request.ContributorIds.Any())
                {
                    query = query.Where(c => request.ContributorIds.Contains(c.ContributorId));
                }
                // Cuối cùng là keyword
                else if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    var keyword = request.Keyword.Trim().ToLower();
                    var unsignedKeyword = StringHelper.RemoveDiacritics(keyword);

                    query = query.Where(c =>
                        c.Title.ToLower().Contains(keyword) ||
                        c.TitleUnsigned.Contains(unsignedKeyword));
                }

               
                var dtoQuery = query.ProjectTo<ContributionSearchResponse>(mapper.ConfigurationProvider);

               
                dtoQuery = dtoQuery.OrderBy(x => Guid.NewGuid());

                // Lấy các bài liên quan trước
                var related = await dtoQuery.Take(request.Quantity).ToListAsync();

                // Nếu chưa đủ số lượng thì bù thêm random từ pool toàn bộ (ngoại trừ bài hiện tại + đã có)
                if (related.Count < request.Quantity)
                {
                    var excludeIds = related.Select(r => r.Id).ToList();
                    if (request.contributionId.HasValue)
                        excludeIds.Add(request.contributionId.Value);

                    var remainingNeeded = request.Quantity - related.Count;

                    var fallbackQuery = initQuery
                        .Where(c => !excludeIds.Contains(c.Id))
                        .ProjectTo<ContributionSearchResponse>(mapper.ConfigurationProvider)
                        .OrderBy(x => Guid.NewGuid());

                    var fallback = await fallbackQuery.Take(remainingNeeded).ToListAsync();

                    related.AddRange(fallback);
                }

                return related;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting related contributions");
                throw;
            }
        }

        public async Task<ContributionOverviewResponse> GetContributionOverview(int id)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;

            if (accountIdClaim == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
      
            var currentContributor = await contributorRepository.GetContributorByUserId(int.Parse(accountIdClaim));
            if (currentContributor == null)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
            }

            var existingContribution = await contributionRepository.GetContributionById(id);

            if (existingContribution == null)
                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);

            if(existingContribution.ContributorId != currentContributor.Id)
            {
                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);
            }

            var response = mapper.Map<ContributionOverviewResponse>(existingContribution);

            response.Note = existingContribution.ContributionAcceptances?
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefault()?.Note;

            var monthlyViews = existingContribution.ContributionAccessLogs
                .GroupBy(log => new { log.CreatedAt.Year, log.CreatedAt.Month })
                .Select(g => new MonthlyViewStat
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Views = g.Count()
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Take(6)
                .OrderBy(x => x.Year).ThenBy(x => x.Month) 
                .ToList();

            response.MonthlyViews = monthlyViews;

            return response;
        }

        public async Task<PageResponse<ContributionOverviewListItemResponse>> GetListContributionsOverview(ContributionOverviewSearchRequest request)
        {
            try
            {        
                var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;

                if (accountIdClaim == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }

                var currentContributor = await contributorRepository.GetContributorByUserId(int.Parse(accountIdClaim));
                if (currentContributor == null)
                {
                    throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
                }

                var query = contributionRepository.GetContributionsByContributorIdQueryable(currentContributor.Id);

                if (request.ContributionStatus.HasValue)
                {
                    query = query.Where(h => h.Status == request.ContributionStatus);
                }

                // Keyword search
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var searchTerm = request.Keyword.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                    query = query.Where(h =>
                      
                        h.Title.ToLower().Contains(searchTerm) ||
                        h.TitleUnsigned.Contains(unsignedTerm) ||

                     
                        h.Contributor.User.UserName.ToLower().Contains(searchTerm) ||
                        h.Contributor.User.UserNameUnsigned.Contains(unsignedTerm) ||
                     
                        h.ContributionHeritageTags.Any(tag =>
                            tag.Heritage.Name.ToLower().Contains(searchTerm) ||
                            tag.Heritage.NameUnsigned.Contains(unsignedTerm)
                        )
                    );
                }


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
                        query = query.OrderByDescending(h => h.CreatedAt);
                        break;
                }


                var dtoQuery = query.ProjectTo<ContributionOverviewListItemResponse>(mapper.ConfigurationProvider);

                // Pagination
                var response = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching contributions");
                throw;
            }
        }

        public async Task<ContributionDetailUpdatedResponse> GetContributionDetailForUpdated(int id)
        {
            var existingContribution = await contributionRepository.GetContributionByIdAndStatus(id, ContributionStatus.PENDING);
            if (existingContribution == null)
                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);

            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;

            if (accountIdClaim == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var currentContributor = await contributorRepository.GetContributorByUserId(int.Parse(accountIdClaim));
            if (currentContributor == null)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
            }
            if (existingContribution.ContributorId != currentContributor.Id)
            {
                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);
            }

            var response = mapper.Map<ContributionDetailUpdatedResponse>(existingContribution);
               
            return response;
        }

        public async Task<ContributionResponse> UpdateContribution(ContributionUpdateRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var currentContributor = await contributorRepository
                .GetContributorByUserId(int.Parse(accountIdClaim));
            if (currentContributor == null)
                throw new AppException(ErrorCode.UNAUTHORIZED);

            var contribution = await contributionRepository.GetContributionByIdAndStatus(request.Id, ContributionStatus.PENDING);
            if (contribution == null)
                throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);

         
            if (contribution.ContributorId != currentContributor.Id)
                throw new AppException(ErrorCode.FORBIDDEN);


            if (!currentContributor.IsPremiumEligible &&
                request.PremiumType == PremiumType.SUBSCRIPTIONONLY)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_IS_NOT_PREMIUM_ELIGIBLE);
            }

            // Update fields
            contribution.Title = request.Title;
            contribution.Content = request.Content;
            contribution.MediaUrl = request.MediaUrl;
            contribution.PremiumType = request.PremiumType;

            // Regenerate
            contribution.PreviewContent = DeltaHelper.GeneratePreviewDelta(contribution.Content);
            contribution.FirstContent = DeltaHelper.ExtractFirstLongParagraph(contribution.Content);
            contribution.GenerateUnsignedFields();

            // Update tags
            contribution.ContributionHeritageTags.Clear();
            if (request.TagHeritageIds?.Any() == true)
            {
                foreach (var heritageId in request.TagHeritageIds)
                {
                    contribution.ContributionHeritageTags.Add(new ContributionHeritageTag
                    {
                        HeritageId = heritageId,
                        ContributionId = contribution.Id
                    });
                }
            }

            await contributionRepository.UpdateAsync(contribution);

            return mapper.Map<ContributionResponse>(contribution);
        }

        private async Task<int?> GetNextStaffForContributionAsync()
        {
            var staffList = await staffRepository.GetActiveReviewersAsync();
            if (!staffList.Any()) return null;

            // Đếm số pending contribution của từng staff
            var staffLoad = staffList
                .Select(s => new
                {
                    StaffId = s.Id,
                    PendingCount = s.ContributionAcceptances
                                    .Count(ca => ca.Status == ContributionStatus.PENDING)
                })
                .ToList();

            var minPending = staffLoad.Min(x => x.PendingCount);

            var candidateStaff = staffLoad
                .Where(x => x.PendingCount == minPending)
                .Select(x => x.StaffId)
                .OrderBy(x => x)
                .ToList();

            var lastAssignedStaffId = await staffRepository.GetLastAssignedStaffIdAsync();

            int selectedStaffId;
            if (lastAssignedStaffId != null && candidateStaff.Contains(lastAssignedStaffId.Value))
            {
                var idx = candidateStaff.IndexOf(lastAssignedStaffId.Value);
                selectedStaffId = candidateStaff[(idx + 1) % candidateStaff.Count];
            }
            else
            {
                selectedStaffId = candidateStaff.First();
            }

            return selectedStaffId;
        }
    }
}
