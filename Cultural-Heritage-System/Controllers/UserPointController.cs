using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.User;
using Cultural_Heritage_System.Dtos.Request.UserPoint;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Dtos.Response.User;
using Cultural_Heritage_System.Dtos.Response.UserPoint;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/user_point")]
    [ApiController]
    public class UserPointController : ControllerBase
    {

        private readonly IMailService mailService;
        private readonly IUserPointService userPointService;

        public UserPointController(IMailService mailService, IUserPointService userPointService)
        {
        
            this.mailService = mailService;
            this.userPointService = userPointService;
        }

        [HttpPost("trade_point_contribution")]
        [Authorize]
        public async Task<ApiResponse<ContributionResponse>> TradePointToUnlockContribution([FromBody] PointToUnlockTokenRequest request)
        {
            var result = await userPointService.TradePointToUnlockContribution(request);

            return new ApiResponse<ContributionResponse>(
                code: 201,
                message: "Trade successfully",
                result: result
            );
        }

        [HttpPost("trade_point_scene")]
        [Authorize]
        public async Task<ApiResponse<PanoramaSceneResponse>> TradePointToUnlockScene(PointToUnlockTokenRequest request)
        {
            var result = await userPointService.TradePointToUnlockScene(request);

            return new ApiResponse<PanoramaSceneResponse>(
                code: 201,
                message: "Trade successfully",
                result: result
            );
        }

    }
}
