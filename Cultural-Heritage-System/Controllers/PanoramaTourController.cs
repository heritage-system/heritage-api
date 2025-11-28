using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Panorama;
using Cultural_Heritage_System.Dtos.Request.Staff;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/panorama_tours")]
    [ApiController]
    public class PanoramaTourController : ControllerBase
    {

        private readonly IPanoramaTourService panoramaTourService;
       

        public PanoramaTourController(IPanoramaTourService panoramaTourService)
        {
            this.panoramaTourService = panoramaTourService;       
        }

        [HttpGet("search_panorama_tour")]
        public async Task<ApiResponse<PageResponse<PanoramaTourSearchResponse>>> GetAllWithSearch(
           [FromQuery] PanoramaTourSearchRequest request)

        {
            return new ApiResponse<PageResponse<PanoramaTourSearchResponse>>
            {
                code = 200,
                result = await panoramaTourService.SearchPanoramaTour(request)
            };
        }

        [HttpPost("create")]     
        public async Task<ApiResponse<bool>> Create([FromBody] PanoramaTourCreationRequest request)
        {
            var result = await panoramaTourService.CreatePanoramaTour(request);

            return new ApiResponse<bool>(
                code: 201,
                message: "Panorama tour created successfully",
                result: result
            );
        }

        [HttpGet("get_panorama_tour_detail")]
        public async Task<ApiResponse<PanoramaTourDetailResponse>> GetPanoramaTourDetail(long id)
        {
            var result = await panoramaTourService.GetPanoramaTourDetail(id);
            return new ApiResponse<PanoramaTourDetailResponse>(
                code: 200,
                message: "Get panorama tour details successfully",
                result: result
            );
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


        [HttpPut("{id}/update")]
        public async Task<ApiResponse<bool>> UpdatePanoramaTour(long id, [FromBody] PanoramaTourCreationRequest request)
        {
            var result = await panoramaTourService.UpdatePanoramaTour(id, request);
            return new ApiResponse<bool>(
                code: 200,
                message: "Updated panorama tour successfully",
                result: result
            );
        }

        [HttpDelete("{id}/delete")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<long?>> DeletePanoramaTour(long id)
        {

            return new ApiResponse<long?>(
                code: 200,
                message: "Delete panorama tour successfully",
                result: await panoramaTourService.DeletePanoramaTour(id)
            );
        }

        [HttpGet("get_panorama_tour")]
        public async Task<ApiResponse<PageResponse<PanoramaTourSearchForAdminResponse>>> GetListPanoramaTourForAdmin(
           [FromQuery] PanoramaTourSearchRequest request)

        {
            return new ApiResponse<PageResponse<PanoramaTourSearchForAdminResponse>>
            {
                code = 200,
                result = await panoramaTourService.GetListPanoramaTourForAdmin(request)
            };
        }

        [HttpGet("get_panorama_tour_detail_for_admin")]
        public async Task<ApiResponse<PanoramaTourDetailForAdminResponse>> GetPanoramaTourDetailForAdmin(long id)
        {
            var result = await panoramaTourService.GetPanoramaTourDetailForAdmin(id);
            return new ApiResponse<PanoramaTourDetailForAdminResponse>(
                code: 200,
                message: "Get panorama tour details successfully",
                result: result
            );
        }

        [HttpPost("create_scene")]
        public async Task<ApiResponse<long>> CreateScene([FromBody] PanoramaSceneCreationRequest request)
        {
            var result = await panoramaTourService.CreatePanoramaScene(request);

            return new ApiResponse<long>(
                code: 201,
                message: "Panorama tour created successfully",
                result: result
            );
        }

        [HttpPut("{id}/update_scene")]
        public async Task<ApiResponse<bool>> UpdatePanoramaScene(long id, [FromBody] PanoramaSceneCreationRequest request)
        {
            var result = await panoramaTourService.UpdatePanoramaScene(id, request);
            return new ApiResponse<bool>(
                code: 200,
                message: "Updated panorama tour successfully",
                result: result
            );
        }

        [HttpDelete("{id}/delete_scene")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<long?>> DeletePanoramaScene(long id)
        {

            return new ApiResponse<long?>(
                code: 200,
                message: "Delete panorama tour successfully",
                result: await panoramaTourService.DeletePanoramaScene(id)
            );
        }
    }
}
