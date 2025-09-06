using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ReportReplyService : IReportReplyService
    {
        private readonly ReportReplyRepository _replyRepository;
        private readonly ILogger<ReportReplyService> _logger;

        public ReportReplyService(ReportReplyRepository replyRepository, ILogger<ReportReplyService> logger)
        {
            _replyRepository = replyRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ReportReply>> GetAllAsync()
        {
            return await _replyRepository.GetAllAsync();
        }

        public async Task<ReportReply> GetByIdAsync(long id)
        {
            return await _replyRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ReportReply>> GetRepliesByReportIdAsync(long reportId)
        {
            return await _replyRepository.GetRepliesByReportIdAsync(reportId);
        }
    }

}
