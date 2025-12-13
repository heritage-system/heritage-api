using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.User;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Dtos.Response.User;
using Cultural_Heritage_System.Dtos.Response.GameMatchHistory;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/match_history")]
    [ApiController]
    public class GameMatchHistoryController : ControllerBase
    {

        private readonly IMailService mailService;
        private readonly IGameMatchHistoryService gameMatchHistoryService;

        public GameMatchHistoryController(IMailService mailService, IGameMatchHistoryService gameMatchHistoryService)
        {
        
            this.mailService = mailService;
            this.gameMatchHistoryService = gameMatchHistoryService;
        }

        [HttpGet("match_history")]
        [Authorize]
        public async Task<ApiResponse<List<UserMatchHistoryResponse>>> GetUserGameMatchHistory()
        {
            var result = await gameMatchHistoryService.GetUserGameMatchHistory();

            return new ApiResponse<List<UserMatchHistoryResponse>>(
                code: 200,
                message: "Get history successfully",
                result: result
            );
        }
   

    }
}
