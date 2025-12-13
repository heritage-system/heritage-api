using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportReplyController : ControllerBase
    {
        private readonly IReportReplyService _reportReplyService;

        public ReportReplyController(IReportReplyService reportReplyService)
        {
            _reportReplyService = reportReplyService;
        }

        [HttpGet("by-reportId")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> GetRepliesByReportId(long reportId)
        {
            var replies = await _reportReplyService.GetRepliesByReportIdAsync(reportId);
            if (replies == null || !replies.Any())
            {
                return NotFound(new { code = 404, message = "Không tìm thấy danh sách trả lời." });
            }
            return Ok(new { code = 200, message = "Lấy danh sách trả lời thành công", result = replies });
        }
    }
}
