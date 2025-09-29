using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
using Cultural_Heritage_System.Dtos.Response.Heritage;
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
        public async Task<ApiResponse<PageResponse<HeritageResponse>>> GetAll([FromQuery] int page,[FromQuery] int pageSize,[FromQuery] string? keyword = null,
                                                                                [FromQuery] int? categoryId = null,[FromQuery] int? tagId = null)
        {
            var heritages = await _heritageService.GetAllAsync(page, pageSize, keyword, categoryId, tagId);

            return new ApiResponse<PageResponse<HeritageResponse>>(
                code: 200,
                message: "Get the heritage list successfully",
                result: heritages
            );
        }

        [HttpGet("get_list_heritage_name")]
        //[Authorize(Roles = "CONTRIBUTOR")]
        public async Task<ApiResponse<List<HeritageNameSearchResponse>>> GetListHeritageName([FromQuery] string keyword)
        {
            var heritages = await _heritageService.SearchListHeritageName(keyword);

            return new ApiResponse<List<HeritageNameSearchResponse>>(
                code: 200,
                message: "Get the heritage's name list successfully",
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
        public async Task<ApiResponse<HeritageResponse>> Create([FromBody] HeritageCreateRequest request)
        {        
            var newHeritage = await _heritageService.CreateAsync(request);

            return new ApiResponse<HeritageResponse>(
                code: 201,
                message: "Heritage created successfully",
                result: newHeritage
            );
        }



        [HttpPut("update")]
        //[Authorize(Roles ="ADMIN")]
        public async Task<ApiResponse<HeritageResponse>> Update([FromBody] HeritageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<HeritageResponse>(
                    code: 400,
                    message: "Invalid model state",
                    result: null
                );
            }

            var updatedHeritage = await _heritageService.UpdateAsync(request);
        
            return new ApiResponse<HeritageResponse>(
                code: 200,
                message: "Heritage updated successfully",
                result: updatedHeritage
            );
        }


        [HttpDelete("delete")]
        //[Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<long?>> Delete([FromQuery] long id)
        {
            var deletedId = await _heritageService.DeleteAsync(id);

            if (deletedId == null)
            {
                return new ApiResponse<long?>(
                    code: 404,
                    message: "Heritage not found",
                    result: null
                );
            }

            return new ApiResponse<long?>(
                code: 200,
                message: "Delete heritage successfully",
                result: deletedId
            );
        }


    }
}
