using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.User;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.User;
using Cultural_Heritage_System.Dtos.Response.UserPoint;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IUserService userService;
        private readonly IMailService mailService;
        private readonly IUserPointService userPointService;

        public UsersController(IUserService userService, IMailService mailService, IUserPointService userPointService)
        {
            this.userService = userService;
            this.mailService = mailService;
            this.userPointService = userPointService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<UserCreationResponse>> CreateUser([FromBody] UserCreationRequest request)
        {
            var users = await userService.CreateUser(request);

            return new ApiResponse<UserCreationResponse>(
                code: 201,
                message: "Created user",
                result: users
            );
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ApiResponse<object>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await userService.ChangePassword(request);
            return new ApiResponse<object>
            {
                code = 200,
                message = "Change password successfully"
            };
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ApiResponse<UpdateProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var result = await userService.UpdateProfile(request);
            return new ApiResponse<UpdateProfileResponse>(
                code: 200,
                message: "Profile updated successfully",
                result: result
            );
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<ApiResponse<UpdateProfileResponse>> GetProfile()
        {
            var result = await userService.GetProfile();
            return new ApiResponse<UpdateProfileResponse>(
                code: 200,
                message: "Profile fetched successfully",
                result: result
            );
        }

        [HttpGet("search_member")]
        //[Authorize]
        public async Task<ApiResponse<PageResponse<UserSearchResponse>>> SearchMemberForAdmin(
           [FromQuery] UserSearchRequest request)

        {
            return new ApiResponse<PageResponse<UserSearchResponse>>
            {
                code = 200,
                result = await userService.SearchMemberForAdmin(request)
            };
        }

        [HttpPost("create_user")]
        [Authorize]
        public async Task<ApiResponse<UserCreationResponse>> CreateUserForAdmin([FromBody] UserCreationByAdminRequest request)
        {
            var users = await userService.CreateUserForAdmin(request);

            return new ApiResponse<UserCreationResponse>(
                code: 201,
                message: "Created user",
                result: users
            );
        }

        [HttpGet("user_detail")]
        public async Task<ApiResponse<UserDetailResponse>> GetUserDetailForAdmin(int id)
        {
            var result = await userService.GetUserDetailForAdmin(id);
            return new ApiResponse<UserDetailResponse>(
                code: 200,
                message: "Get user details successfully",
                result: result
            );
        }

        [HttpPut("{id}/status")]
        public async Task<ApiResponse<bool>> ChangeUserStatusForAdmin(int id, [FromBody] UserStatus status)
        {
            var result = await userService.ChangeUserStatusForAdmin(id, status);
            return new ApiResponse<bool>(
                code: 200,
                message: "Change user status successfully",
                result: result
            );
        }

        [HttpGet("remind_mail")]
        public async Task<ApiResponse<bool>> SendRemindMailTest()
        {
            var vnZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            var scheduleVNTime = DateTime.Now.AddMinutes(2);

            // Convert Local → Unspecified để tránh lỗi Kind
            var vnUnspecified = DateTime.SpecifyKind(scheduleVNTime, DateTimeKind.Unspecified);

            // Convert VN → UTC
            var scheduleUtc = TimeZoneInfo.ConvertTimeToUtc(vnUnspecified, vnZone);


            await mailService.SendRemindEmail(
                to: "ginokami24@gmail.com",
                userName: "Thịnh",
                eventName: "Hội thảo Online",
                startTime: "19:00",
                eventDate: "05/12/2025",
                joinUrl: "https://vtfp.com/join/xyz",
                scheduleTimeUtc: scheduleUtc
            );
            return new ApiResponse<bool>(
                code: 200,
                message: "Change user status successfully",
                result: true
            );
        }

        [HttpGet("user_point")]
        public async Task<ApiResponse<UserPointResponse>> GetUserPoint()
        {
            var result = await userPointService.GetUserPointByUserId();
            return new ApiResponse<UserPointResponse>(
                code: 200,
                message: "Get user point successfully",
                result: result
            );
        }
    }
}
