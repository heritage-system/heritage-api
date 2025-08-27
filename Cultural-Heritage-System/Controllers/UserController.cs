using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
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
        private readonly ITestSearchService testSearchService;

        public UsersController(IUserService userService, ITestSearchService testSearchService)
        {
            this.userService = userService;
            this.testSearchService = testSearchService;
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


        [HttpGet("search_heritage")]
        public async Task<ApiResponse<PageResponse<HeritageSearchResponse>>> GetAllWithSearch(
           [FromQuery] HeritageSearchRequest request)

        {
            return new ApiResponse<PageResponse<HeritageSearchResponse>>
            {
                code = 200,
                result = await testSearchService.SearchHeritagesAsync(request)
            };
        }

    }
}
