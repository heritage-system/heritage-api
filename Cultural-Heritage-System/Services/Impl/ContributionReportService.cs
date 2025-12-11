using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.ContributionReport;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ContributionReportService : IContributionReportService
    {
        private readonly IContributionReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly IContributionReportReplyRepository _reportReplyRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ContributionReportService> _logger;

        public ContributionReportService(IContributionReportRepository reportRepository, IMapper mapper, IUserRepository userRepository, IMailService mailService, IContributionReportReplyRepository reportReplyRepository, IStaffRepository staffRepository, IHttpContextAccessor httpContextAccessor, ILogger<ContributionReportService> logger)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _mailService = mailService;
            _reportReplyRepository = reportReplyRepository;
            _staffRepository = staffRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<PageResponse<ContributionReportResponse>> GetAllAsync(int page, int pageSize, string? keyword = null, DateTime? startDate = null, DateTime? endDate = null, string? status = null)
        {
            var query = _reportRepository.GetAllQuery().AsNoTracking();

            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(lowerKeyword);

                query = query.Where(r =>
                    r.Contribution.Title.ToLower().Contains(lowerKeyword) ||
                    r.Contribution.TitleUnsigned.ToLower().Contains(unsignedTerm) ||
                    r.User.UserName.ToLower().Contains(lowerKeyword) ||
                    r.User.UserNameUnsigned.ToLower().Contains(unsignedTerm));
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {              
                    query = query.Where(r => r.Status.ToString() == status);            
            }


            var pagedResult = await query.ToPagedResponseAsync(page, pageSize);

            return _mapper.Map<PageResponse<ContributionReportResponse>>(pagedResult);
        }

        public async Task<ContributionReportResponse?> GetByIdAsync(long id)
        {
            var entity = await _reportRepository.GetByIdAsync(id);
            return _mapper.Map<ContributionReportResponse?>(entity);
        }   

        public async Task<bool> AnswerContributionReportAsync(long reportId, string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                throw new AppException(ErrorCode.FILE_INVALID);
            }

            var currentUserId = GetCurrentUserId();
            if (currentUserId == null || !currentUserId.HasValue)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var staff = await _staffRepository.GetStaffByUserId(currentUserId.Value);
            if (staff == null)
            {
                throw new AppException(ErrorCode.FORBIDDEN);
            }

            if (!staff.CanReplyReports)
            {
                throw new AppException(ErrorCode.FORBIDDEN);
            }

            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null)
            {
                return false;
            }

            var user = await _userRepository.GetByIdAsync(report.UserId);
            if (user == null || string.IsNullOrWhiteSpace(user.Email))
            {
                return false;
            }

            var reply = new ContributionReportReply
            {
                ContributionReportId = reportId,
                StaffId = staff.Id,
                Message = answer.Trim(),
            };
            await _reportReplyRepository.AddAsync(reply);

            if (report.Status == ReportStatus.PENDING)
            {
                report.Status = ReportStatus.ANSWERED;
                await _reportRepository.UpdateAsync(report);
            }

            try
            {
                var createdAtFormatted = report.CreatedAt.ToString("dd/MM/yyyy HH:mm");

                await _mailService.SendEmailAnswerReport(
                    user.Email,
                    user.UserName,
                    report.Contribution.Title,
                    createdAtFormatted,
                    report.Reason,
                    answer
                );

                _logger.LogInformation("Answer email sent successfully to {Email} for report {ContributionReportId}", user.Email, reportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send answer email for report {ContributionReportId} to {Email}. Reply was saved successfully but email notification failed.", reportId, user.Email);
            }

            return true;
        }



        private int? GetCurrentUserId()
        {
            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return null;
            }

            if (int.TryParse(accountIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
