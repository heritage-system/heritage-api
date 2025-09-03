using AutoMapper;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ReportService : IReportService
    {
        private readonly ReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        private readonly IMailService _mailService;

        public ReportService(ReportRepository reportRepository, IMapper mapper, UserRepository userRepository, IMailService mailService)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _mailService = mailService;
        }

        public async Task<IEnumerable<ReportResponse>> GetAllAsync()
        {
            var entities = await _reportRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReportResponse>>(entities);
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
            return true;
        }
    }
}
