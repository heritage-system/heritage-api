using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Services.Impl
{
    public class ContributionReportReplyService : IContributionReportReplyService
    {
        private readonly IContributionReportReplyRepository _replyRepository;
        private readonly ILogger<ContributionReportReplyService> _logger;

        public ContributionReportReplyService(IContributionReportReplyRepository replyRepository, ILogger<ContributionReportReplyService> logger)
        {
            _replyRepository = replyRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ContributionReportReply>> GetAllAsync()
        {
            return await _replyRepository.GetAllAsync();
        }

        public async Task<ContributionReportReply> GetByIdAsync(long id)
        {
            return await _replyRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ContributionReportReply>> GetRepliesByContributionReportIdAsync(long reportId)
        {
            return await _replyRepository.GetRepliesByReportIdAsync(reportId);
        }
    }

}
