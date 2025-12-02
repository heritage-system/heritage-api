using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly IReportReplyRepository _reportReplyRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IReportRepository reportRepository, IMapper mapper, IUserRepository userRepository, IMailService mailService, IReportReplyRepository reportReplyRepository, IStaffRepository staffRepository, IHttpContextAccessor httpContextAccessor, ILogger<ReportService> logger)
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

        public async Task<PageResponse<ReportResponse>> GetAllAsync(int page,int pageSize,string? keyword = null,DateTime? startDate = null,DateTime? endDate = null,string? status = null) 
        {
            var query = _reportRepository.GetAllQuery();

            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(lowerKeyword);

                query = query.Where(r =>
                    r.Heritage.Name.ToLower().Contains(lowerKeyword) ||
                    r.Heritage.NameUnsigned.ToLower().Contains(unsignedTerm) ||
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
                if (Enum.TryParse<ReportStatus>(status, true, out var parsedStatus))
                {
                    query = query.Where(r => r.Status == parsedStatus);
                }
            }


            var pagedResult = await query.ToPagedResponseAsync(page, pageSize);

            return _mapper.Map<PageResponse<ReportResponse>>(pagedResult);
        }

        public async Task<ReportResponse?> GetByIdAsync(long id)
        {
            var entity = await _reportRepository.GetByIdAsync(id);
            return _mapper.Map<ReportResponse?>(entity);
        }

        public async Task<ReportResponse> CreateAsync(CreateReportRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            request.UserId = userId.Value; 
            var entity = _mapper.Map<Report>(request);
            await _reportRepository.AddAsync(entity);
            return _mapper.Map<ReportResponse>(entity);
        }


        public async Task<bool> AnswerReportAsync(long reportId, string answer)
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

            var reply = new ReportReply
            {
                ReportId = reportId,
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
                await _mailService.SendEmailAnswerReport(user.Email, reportId, answer);
                _logger.LogInformation("Answer email sent successfully to {Email} for report {ReportId}", user.Email, reportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send answer email for report {ReportId} to {Email}. Reply was saved successfully but email notification failed.", reportId, user.Email);
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
