using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Cultural_Heritage_System.Dtos.Response.Panorama;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/panoramaTours")]
    [ApiController]
    public class PanoramaTourController : ControllerBase
    {

        private readonly IPanoramaTourService panoramaTourService;
       

        public PanoramaTourController(IPanoramaTourService panoramaTourService)
        {
            this.panoramaTourService = panoramaTourService;       
        }

        

        [HttpGet("get_panorama_scene_detail")]
        public async Task<ApiResponse<PanoramaSceneResponse>> GetPanoramaSceneDetail(long id)
        {
            var result = await panoramaTourService.GetPanoramaSceneDetail(id);
            return new ApiResponse<PanoramaSceneResponse>(
                code: 200,
                message: "Get panorama scene details successfully",
                result: result
            );
        }
      
    }
}
