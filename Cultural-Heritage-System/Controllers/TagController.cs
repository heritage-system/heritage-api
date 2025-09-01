using Cultural_Heritage_System.Dtos.Request.Tag;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Tag;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/Tags")]
    [ApiController]
    public class TagController : ControllerBase
    {

        private readonly ITagService TagService;

        public TagController(ITagService TagService)
        {
            this.TagService = TagService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<CreateTagResponse>> CreateTag([FromBody] CreateTagRequest request)
        {
            var Tags = await TagService.CreateTag(request);

            return new ApiResponse<CreateTagResponse>(
                code: 200,
                message: "Created Tag",
                result: Tags
            );
        }
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<UpdateTagResponse>> UpdateTag([FromBody] UpdateTagRequest request)
        {
            var Tags = await TagService.UpdateTag(request);

            return new ApiResponse<UpdateTagResponse>(
                code: 200,
                message: "Update Tag",
                result: Tags
            );
        }
        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<DeleteTagResponse>> DeleteTag([FromBody] DeleteTagRequest request)
        {
            var Tags = await TagService.DeleteTag(request);

            return new ApiResponse<DeleteTagResponse>(
                code: 200,
                message: "Delete Tag"
            );
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<IQueryable<Tag>>> GetTags()
        {
            var Tags = TagService.GetTagsQueryable();

            return new ApiResponse<IQueryable<Tag>>(
                code: 200,
                message: "get Tags",
                result: Tags
            );
        }
        [HttpGet("search_tag")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ApiResponse<PageResponse<TagSearchResponse>>> GetAllWithSearch(
          [FromQuery] TagSearchRequest request)

        {
            return new ApiResponse<PageResponse<TagSearchResponse>>
            {
                code = 200,
                result = await TagService.SearchTagsAsync(request)
            };
        }

    }
}
