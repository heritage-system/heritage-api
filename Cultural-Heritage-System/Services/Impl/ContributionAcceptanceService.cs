using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.AccessControl;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ContributionAcceptanceService : IContributionAcceptanceService
    {
        private readonly IContributionAcceptanceRepository _contributionAcceptanceRepository;
        private readonly IContributionRepository _contributionRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContributionAcceptanceService(
            IContributionAcceptanceRepository contributionAcceptanceRepository,
            IContributionRepository contributionRepository,
            IStaffRepository staffRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _contributionAcceptanceRepository = contributionAcceptanceRepository;
            _contributionRepository = contributionRepository;
            _staffRepository = staffRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task ApproveOrRejectAsync(long acceptanceId, ContributionStatus status, string? note = null)
        {
            var acceptance = await _contributionAcceptanceRepository.GetByIdAsync(acceptanceId)
                ?? throw new AppException(ErrorCode.ACCEPTANCE_NOT_FOUND);

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            int currentUserId = int.Parse(userIdClaim);
            var currentStaff = await _staffRepository.GetByUserIdAsync(currentUserId)
                ?? throw new AppException(ErrorCode.FORBIDDEN);

            if (acceptance.StaffId != currentStaff.Id)
                throw new AppException(ErrorCode.FORBIDDEN);

            if (status == ContributionStatus.REJECTED && string.IsNullOrWhiteSpace(note))
                throw new AppException(ErrorCode.NOTE_REQUIRED_WHEN_REJECT);

            acceptance.Status = status;
            acceptance.Note = note?.Trim();
            acceptance.AcceptedAt = DateTimeOffset.UtcNow;

            await _contributionAcceptanceRepository.UpdateAsync(acceptance);

            var allReviewers = await _contributionAcceptanceRepository.GetAllByContributionIdAsync(acceptance.ContributionId);
            var contribution = acceptance.Contribution!;

            if (allReviewers.Any(a => a.Status == ContributionStatus.REJECTED))
            {
                contribution.Status = ContributionStatus.REJECTED;
            }
            else if (allReviewers.All(a => a.Status == ContributionStatus.APPROVED))
            {
                contribution.Status = ContributionStatus.APPROVED;
                contribution.ApprovedAt = DateTimeOffset.UtcNow;
            }
            else
            {
                contribution.Status = ContributionStatus.PENDING;
            }

            await _contributionRepository.UpdateAsync(contribution);
        }

        public async Task<ContributionOverviewResponse> GetContributionOverviewForStaff(int contributionId)
        {
            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (accountIdClaim == null)
                throw new AppException(ErrorCode.UNAUTHORIZED);

            int currentUserId = int.Parse(accountIdClaim);

            var currentStaff = await _staffRepository.GetByUserIdAsync(currentUserId)
                ?? throw new AppException(ErrorCode.FORBIDDEN);

            bool isAssigned = await _contributionAcceptanceRepository.IsAssignedToStaffAsync(contributionId, currentStaff.Id);
            if (!isAssigned)
                throw new AppException(ErrorCode.FORBIDDEN);

            var contribution = await _contributionRepository.GetContributionForStaffAsync(contributionId, currentStaff.Id)
                ?? throw new AppException(ErrorCode.CONTRIBUTION_NOT_EXISTED);

            var response = _mapper.Map<ContributionOverviewResponse>(contribution);

            var monthlyViews = contribution.ContributionAccessLogs
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
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            response.MonthlyViews = monthlyViews;
            return response;
        }

        public async Task<PageResponse<ContributionOverviewListItemResponse>> GetListContributionsOverviewForStaff(ContributionOverviewSearchRequest request)
        {
            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (accountIdClaim == null)
                throw new AppException(ErrorCode.UNAUTHORIZED);

            int userId = int.Parse(accountIdClaim);

            var staff = await _staffRepository.GetByUserIdAsync(userId)
                ?? throw new AppException(ErrorCode.FORBIDDEN);

            var query = _contributionRepository.GetContributionsByStaffIdQueryable(staff.Id);

            if (request.ContributionStatus.HasValue)
                query = query.Where(h => h.Status == request.ContributionStatus);

            if (!string.IsNullOrWhiteSpace(request.Keyword))
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

            query = request.SortBy switch
            {
                SortBy.IDASC => query.OrderBy(h => h.Id),
                SortBy.IDDESC => query.OrderByDescending(h => h.Id),
                SortBy.NAMEASC => query.OrderBy(h => h.Title),
                SortBy.NAMEDESC => query.OrderByDescending(h => h.Title),
                _ => query.OrderByDescending(h => h.CreatedAt)
            };

            var dtoQuery = query.ProjectTo<ContributionOverviewListItemResponse>(_mapper.ConfigurationProvider);
            return await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);
        }
    }
}
