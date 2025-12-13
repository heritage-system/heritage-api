using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Staff;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Staff;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/staffs")]
    [ApiController]
    public class StaffsController : ControllerBase
    {

        private readonly IStaffService staffService;
      
        public StaffsController(IStaffService staffService)
        {
            this.staffService = staffService;
           
        }
     
        [HttpGet("search_staff")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<PageResponse<StaffSearchResponse>>> SearchStaffForAdmin(
           [FromQuery] StaffSearchRequest request)

        {
            return new ApiResponse<PageResponse<StaffSearchResponse>>
            {
                code = 200,
                result = await staffService.SearchStaffForAdmin(request)
            };
        }
    
        [HttpGet("staff_detail")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<StaffDetailResponse>> GetStaffDetailForAdmin(int id)
        {
            var result = await staffService.GetStaffDetailForAdmin(id);
            return new ApiResponse<StaffDetailResponse>(
                code: 200,
                message: "Get staff details successfully",
                result: result
            );
        }

        [HttpPut("{id}/update")]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<bool>> UpdateStaffForAdmin(int id,[FromBody] StaffUpdateRequest request)
        {
            var result = await staffService.UpdateStaffForAdmin(id, request);
            return new ApiResponse<bool>(
                code: 200,
                message: "Change staff status successfully",
                result: result
            );
        }

    }
}
