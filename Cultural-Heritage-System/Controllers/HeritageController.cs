using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeritageController : ControllerBase
    {
        private readonly IHeritageService _heritageService;

        public HeritageController(IHeritageService heritageService)
        {
            _heritageService = heritageService;
        }

        [HttpGet("all")]
        [Authorize(Roles = "MEMBER")]
        public async Task<ApiResponse<PageResponse<HeritageResponse>>> GetAll([FromQuery] int page = 1,[FromQuery] int pageSize = 5,[FromQuery] string? keyword = null,
                                                                                [FromQuery] int? categoryId = null,[FromQuery] int? tagId = null)
        {
            var heritages = await _heritageService.GetAllAsync(page, pageSize, keyword, categoryId, tagId);

            return new ApiResponse<PageResponse<HeritageResponse>>(
                code: 200,
                message: "Get the heritage list successfully",
                result: heritages
            );
        }

        [HttpGet("id")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<HeritageResponse>> GetById([FromQuery] long id)
        {
            var heritage = await _heritageService.GetByIdAsync(id);

            if (heritage == null)
            {
                return new ApiResponse<HeritageResponse>(
                    code: 404,
                    message: "Heritage not found"
                );
            }

            return new ApiResponse<HeritageResponse>(
                code: 200,
                message: "Heritage fetched successfully",
                result: heritage
            );
        }


        [HttpPost("create")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<HeritageResponse>> Create([FromForm] HeritageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<HeritageResponse>(
                    code: 400,
                    message: "Invalid model state",
                    result: null
                );
            }

            var newHeritage = await _heritageService.CreateAsync(request);

            return new ApiResponse<HeritageResponse>(
                code: 201,
                message: "Heritage created successfully",
                result: newHeritage
            );
        }



        [HttpPut("update")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<HeritageResponse>> Update([FromQuery] long id, [FromBody] HeritageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<HeritageResponse>(
                    code: 400,
                    message: "Invalid model state",
                    result: null
                );
            }

            var updatedHeritage = await _heritageService.UpdateAsync(id, request);

            if (updatedHeritage == null)
            {
                return new ApiResponse<HeritageResponse>(
                    code: 404,
                    message: "Heritage not found",
                    result: null
                );
            }

            return new ApiResponse<HeritageResponse>(
                code: 200,
                message: "Heritage updated successfully",
                result: updatedHeritage
            );
        }


        [HttpDelete("delete")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<string>> Delete([FromQuery] long id)
        {
            var result = await _heritageService.DeleteAsync(id);

            if (!result)
            {
                return new ApiResponse<string>(
                    code: 404,
                    message: "Heritage not found"
                );
            }

            return new ApiResponse<string>(
                code: 200,
                message: "Delete heritage successfully"
            );
        }

    }
}
