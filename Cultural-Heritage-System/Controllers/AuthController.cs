using System.Net;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("/api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("sign-in")]
        public async Task<ApiResponse<SignInResponse>> SignIn([FromBody] SignInRequest request)
        {
            var result = await authService.SignIn(request);
            return new ApiResponse<SignInResponse>
            {
                code = ((int)HttpStatusCode.OK),
                message = "Sign-in successful",
                result = result
            };
        }

        [HttpPost("outbound")]
        [AllowAnonymous]
        public async Task<ApiResponse<SignInResponse>> SigInGoogle([FromQuery] string code)
        {
            var result = await authService.SignInWithGoogle(code);

            return new ApiResponse<SignInResponse>
            {
                code = ((int)HttpStatusCode.OK),
                message = "SignIn Google Successfully",
                result = result
            };
        }

        [HttpPost("facebook")]
        [AllowAnonymous]
        public async Task<ApiResponse<SignInResponse>> SigInFacebook([FromQuery] string code)
        {
            var result = await authService.SignInWithFacebook(code);

            return new ApiResponse<SignInResponse>
            {
                code = ((int)HttpStatusCode.OK),
                message = "SignIn Facebook Successfully",
                result = result
            };
        }

    }
}
