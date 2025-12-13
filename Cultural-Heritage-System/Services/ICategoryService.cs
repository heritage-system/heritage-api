using Cultural_Heritage_System.Dtos.Request.Category;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Category;
using Cultural_Heritage_System.Models;


namespace Cultural_Heritage_System.Services
{
    public interface ICategoryService
    {
        Task<CreateCategoryResponse> CreateCategory(CreateCategoryRequest request);
        Task<UpdateCategoryResponse> UpdateCategory(UpdateCategoryRequest request);
        Task<DeleteCategoryResponse> DeleteCategory(DeleteCategoryRequest request);
        IQueryable<Category> GetCategoriesQueryable();
        Task<PageResponse<CategorySearchResponse>> SearchCategoriesAsync(CategorySearchRequest request);
    }
}
