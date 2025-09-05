using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ReportService : IReportService
    {
        private readonly ReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        private readonly IMailService _mailService;
        private readonly ReportReplyRepository _reportReplyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportService(ReportRepository reportRepository, IMapper mapper, UserRepository userRepository, IMailService mailService, ReportReplyRepository reportReplyRepository, IHttpContextAccessor httpContextAccessor)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _mailService = mailService;
            _reportReplyRepository = reportReplyRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PageResponse<ReportResponse>> GetAllAsync(int page, int pageSize, string? keyword = null)
        {
            var query = _reportRepository.GetAllQuery();

            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(lowerKeyword);
                query = query.Where(r => r.Reason.ToLower().Contains(lowerKeyword)
                                      || r.Heritage.Name.ToLower().Contains(lowerKeyword)
                                      || r.User.UserName.ToLower().Contains(lowerKeyword)
                                      || r.User.UserNameUnsigned.ToLower().Contains(unsignedTerm));

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
            var entity = _mapper.Map<Report>(request);
            await _reportRepository.AddAsync(entity);
            return _mapper.Map<ReportResponse>(entity);
        }

        public async Task<ReportResponse?> UpdateAsync(long id, UpdateReportRequest request)
        {
            var entity = await _reportRepository.GetByIdAsync(id);
            if (entity == null) return null;

            if (request.Reason != null)
            {
                entity.Reason = request.Reason;
            }

            await _reportRepository.UpdateAsync(entity);
            return _mapper.Map<ReportResponse>(entity);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var report = await _reportRepository.GetByIdAsync(id);
            if (report == null) return false;
            await _reportRepository.DeleteAsync(report);
            return true;
        }
        public async Task<bool> AnswerReportAsync(long reportId, string answer)
        {
            var report = await _reportRepository.GetByIdAsync(reportId);
            if (report == null) return false;

            var user = await _userRepository.GetByIdAsync(report.UserId);
            if (user == null || string.IsNullOrWhiteSpace(user.Email)) return false;

            await _mailService.SendEmailAnswerReport(user.Email, reportId, answer);

            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);


            var reply = new ReportReply
            {
                ReportId = reportId,
                CreatedBy = accountIdClaim, 
                Message = answer,
            };
            await _reportReplyRepository.AddAsync(reply);

            if (report.Status == ReportStatus.Pending)
            {
                report.Status = ReportStatus.Answered;
                await _reportRepository.UpdateAsync(report);
            }

            return true;
        }

    }
}
