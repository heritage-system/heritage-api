using Cultural_Heritage_System.Dtos.Request.Category;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Category;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/Category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        private readonly ICategoryService CateService;

        public CategoryController(ICategoryService CateService)
        {
            this.CateService = CateService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ApiResponse<CreateCategoryResponse>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var Categories = await CateService.CreateCategory(request);

            return new ApiResponse<CreateCategoryResponse>(
                code: 200,
                message: "Created Category",
                result: Categories
            );
        }
        [HttpPut]
        [AllowAnonymous]
        public async Task<ApiResponse<UpdateCategoryResponse>> UpdateCategory([FromBody] UpdateCategoryRequest request)
        {
            var Categories = await CateService.UpdateCategory(request);

            return new ApiResponse<UpdateCategoryResponse>(
                code: 200,
                message: "Update Category",
                result: Categories
            );
        }
        [HttpDelete]
        [AllowAnonymous]
        public async Task<ApiResponse<DeleteCategoryResponse>> DeleteCategory([FromBody] DeleteCategoryRequest request)
        {
            var Categories = await CateService.DeleteCategory(request);

            return new ApiResponse<DeleteCategoryResponse>(
                code: 200,
                message: "Delete Category"
            );
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ApiResponse<IQueryable<Category>>> GetCategorys()
        {
            var Categories = CateService.GetCategoriesQueryable();

            return new ApiResponse<IQueryable<Category>>(
                code: 200,
                message: "get Categoris",
                result: Categories
            );
        }

    }
}
