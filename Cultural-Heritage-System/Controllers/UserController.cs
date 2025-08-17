using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Services;
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

        public UsersController(IUserService userService)
        {
            this.userService = userService;
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
    }
}
