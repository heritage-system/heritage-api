using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("all")]
        //[Authorize(Roles = "MEMBER")]
        public async Task<ApiResponse<PageResponse<ReportResponse>>> GetAll(
             [FromQuery] int page,
             [FromQuery] int pageSize,
             [FromQuery] string? keyword = null,
             [FromQuery] DateTime? startDate = null,
             [FromQuery] DateTime? endDate = null,
             [FromQuery] string? status = null) 
        {
            var result = await _reportService.GetAllAsync(page, pageSize, keyword, startDate, endDate, status);
            return new ApiResponse<PageResponse<ReportResponse>>(
                code: 200,
                message: "Get reports successfully",
                result: result
            );
        }



        [HttpGet("id")]
        //[Authorize(Roles = "MEMBER")]
        public async Task<ApiResponse<ReportResponse>> GetById([FromQuery] long id)
        {
            var report = await _reportService.GetByIdAsync(id);
            if (report == null)
            {
                return new ApiResponse<ReportResponse>(
                    code: 404,
                    message: "Report not found"
                );
            }

            return new ApiResponse<ReportResponse>(
                code: 200,
                message: "Get report successfully",
                result: report
            );
        }

        [HttpPost("create")]
        //[Authorize(Roles = "MEMBER")]
        public async Task<ApiResponse<bool>> Create([FromBody] CreateReportRequest request)
        {           
            var created = await _reportService.CreateAsync(request);
            return new ApiResponse<bool>(
                code: 201,
                message: "Report created successfully",
                result: created
            );
        }

        [HttpPost("answer")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<object>> Answer([FromBody] AnswerReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<object>(
                    code: 400,
                    message: "Invalid model state"
                );
            }

            var ok = await _reportService.AnswerReportAsync(request.ReportId, request.Answer);
            if (!ok)
            {
                return new ApiResponse<object>(
                    code: 404,
                    message: "Report or user not found"
                );
            }

            return new ApiResponse<object>(
                code: 200,
                message: "Answer sent successfully"
            );
        }
    }
}
