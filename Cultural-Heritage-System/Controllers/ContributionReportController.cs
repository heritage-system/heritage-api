
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.ContributionReport;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributionReportController : ControllerBase
    {
        private readonly IContributionReportService _reportService;

        public ContributionReportController(IContributionReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("all")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<PageResponse<ContributionReportResponse>>> GetAll(
             [FromQuery] int page,
             [FromQuery] int pageSize,
             [FromQuery] string? keyword = null,
             [FromQuery] DateTime? startDate = null,
             [FromQuery] DateTime? endDate = null,
             [FromQuery] string? status = null) 
        {
            var result = await _reportService.GetAllAsync(page, pageSize, keyword, startDate, endDate, status);
            return new ApiResponse<PageResponse<ContributionReportResponse>>(
                code: 200,
                message: "Get reports successfully",
                result: result
            );
        }



        [HttpGet("id")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<ContributionReportResponse>> GetById([FromQuery] long id)
        {
            var report = await _reportService.GetByIdAsync(id);
            if (report == null)
            {
                return new ApiResponse<ContributionReportResponse>(
                    code: 404,
                    message: "ContributionReport not found"
                );
            }

            return new ApiResponse<ContributionReportResponse>(
                code: 200,
                message: "Get report successfully",
                result: report
            );
        }
       
        [HttpPost("answer")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<object>> Answer([FromBody] AnswerContributionReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<object>(
                    code: 400,
                    message: "Invalid model state"
                );
            }

            var ok = await _reportService.AnswerContributionReportAsync(request.ReportId, request.Answer);
            if (!ok)
            {
                return new ApiResponse<object>(
                    code: 404,
                    message: "ContributionReport or user not found"
                );
            }

            return new ApiResponse<object>(
                code: 200,
                message: "Answer sent successfully"
            );
        }
    }
}
