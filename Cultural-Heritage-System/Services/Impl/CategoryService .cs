using AutoMapper;
using Cultural_Heritage_System.Dtos.Request.Category;
using Cultural_Heritage_System.Dtos.Response.Category;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly CategoryRepository cateRepository;

        private readonly IMapper mapper;

        private readonly ILogger<CategoryService> logger;

        public CategoryService(CategoryRepository cateRepository, ILogger<CategoryService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.cateRepository = cateRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateCategoryResponse> CreateCategory(CreateCategoryRequest request)
        {

            var Category = mapper.Map<Category>(request);
            await cateRepository.AddAsync(Category);
            return mapper.Map<CreateCategoryResponse>(Category);
        }

        public async Task<DeleteCategoryResponse> DeleteCategory(DeleteCategoryRequest request)
        {
            var Category = mapper.Map<Category>(request);
            await cateRepository.DeleteAsync(Category);
            return mapper.Map<DeleteCategoryResponse>(Category);
        }
        public async Task<UpdateCategoryResponse> UpdateCategory(UpdateCategoryRequest request)
        {
            var Category = mapper.Map<Category>(request);
            await cateRepository.UpdateAsync(Category);
            return mapper.Map<UpdateCategoryResponse>(Category);
        }
        public IQueryable<Category> GetCategoriesQueryable()
        {
            return cateRepository.GetCategoriesQueryable();
        }


    }
}
