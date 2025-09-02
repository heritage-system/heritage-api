using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Category;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Category;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly CategoryRepository categoryRepository;
        private readonly UserRepository userRepository;
        private readonly IMapper mapper;

        private readonly ILogger<CategoryService> logger;

        public CategoryService(CategoryRepository categoryRepository, UserRepository userRepository, ILogger<CategoryService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.categoryRepository = categoryRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateCategoryResponse> CreateCategory(CreateCategoryRequest request)
        {


            if (string.IsNullOrWhiteSpace(request.Name))
            {
                logger.LogError("Invalid  Name");
                throw new AppException(ErrorCode.INVALID_CATEGORY_NAME);
            }
            var existingCategory = categoryRepository
     .GetCategoriesQueryable()
     .FirstOrDefault(t => t.Name == request.Name);
            if (existingCategory != null)
            {
                logger.LogError("Category Existed");
                throw new AppException(ErrorCode.CATEGORY_EXISTED);
            }
            Category Category = mapper.Map<Category>(request);
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            Category.CreatedBy = accountIdClaim;
            //Category.GenerateUnsignedFields();
            await categoryRepository.AddAsync(Category);
            return mapper.Map<CreateCategoryResponse>(Category);
        }

        public async Task<DeleteCategoryResponse> DeleteCategory(DeleteCategoryRequest request)
        {
            // Fetch from DB
            var Category = await categoryRepository.GetCategoriesQueryable()
                                         .FirstOrDefaultAsync(t => t.Id == request.id);

            if (Category == null)
            {
                logger.LogError("Category Not Existed");
                throw new AppException(ErrorCode.CATEGORY_NOT_EXISTED);
            }
            mapper.Map(request, Category);
            try
            {
                await categoryRepository.DeleteAsync(Category);
            }
            catch(Exception e)
            {
                throw new AppException(ErrorCode.CATEGORY_ALREADY_USED);
            }
            return mapper.Map<DeleteCategoryResponse>(Category);
        }
        public async Task<UpdateCategoryResponse> UpdateCategory(UpdateCategoryRequest request)
        {
            // Fetch from DB
            var Category = await categoryRepository.GetCategoriesQueryable()
                                         .FirstOrDefaultAsync(t => t.Id == request.id);

            if (Category == null)
            {
                logger.LogError("Category Not Existed");
                throw new AppException(ErrorCode.CATEGORY_NOT_EXISTED);
            }
            mapper.Map(request, Category);
            //Category.GenerateUnsignedFields();
            Category.UpdatedAt = DateTime.UtcNow;
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            Category.UpdatedBy = accountIdClaim;
            await categoryRepository.UpdateAsync(Category);
            return mapper.Map<UpdateCategoryResponse>(Category);
        }
        public IQueryable<Category> GetCategoriesQueryable()
        {
            return categoryRepository.GetCategoriesQueryable();
        }

        public async Task<PageResponse<CategorySearchResponse>> SearchCategoriesAsync(CategorySearchRequest request)
        {
            var query = GetCategoriesQueryable();

            // Filter by keyword (both Name and Description, signed & unsigned)
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var searchTerm = request.Keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.NameUnsigned.Contains(unsignedTerm) ||
                    (c.Description != null && c.Description.ToLower().Contains(searchTerm)) ||
                    (c.DescriptionUnsigned != null && c.DescriptionUnsigned.Contains(unsignedTerm))
                );
            }

            // Sorting
            if (request.SortBy.HasValue)
            {
                switch (request.SortBy.Value)
                {
                    case SortBy.NAMEASC:
                        query = query.OrderBy(c => c.Name);
                        break;
                    case SortBy.NAMEDESC:
                        query = query.OrderByDescending(c => c.Name);
                        break;
                }
            }

            // Apply pagination and map to DTO (without CreateByName / UpdatedByName yet)
            var paged = await query
                .Select(c => new CategorySearchResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    NameUnsigned = c.NameUnsigned,
                    DescriptionUnsigned = c.DescriptionUnsigned,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    Count = c.Heritages.Count()
                })
                .ToPagedResponseAsync(request.Page, request.PageSize);

            // Fill in CreateByName and UpdatedByName
            foreach (var item in paged.Items)
            {
                if (!string.IsNullOrEmpty(item.CreatedBy))
                {
                    if (item.CreatedBy == "system")
                    {
                        item.CreateByName = "System";
                        item.CreateByEmail = "system"; // 👈 set email as "system"
                    }
                    else
                    {
                        var user = await userRepository.GetByIdAsync(int.Parse(item.CreatedBy));
                        item.CreateByName = user?.UserName;
                        item.CreateByEmail = user?.Email;
                    }
                }

                if (!string.IsNullOrEmpty(item.UpdatedBy))
                {
                    if (item.UpdatedBy == "system")
                    {
                        item.UpdatedByName = "System";
                        item.UpdatedByEmail = "system"; // 👈 set email as "system"
                    }
                    else
                    {
                        var user = await userRepository.GetByIdAsync(int.Parse(item.UpdatedBy));
                        item.UpdatedByName = user?.UserName;
                        item.UpdatedByEmail = user?.Email;
                    }
                }
            }

            return paged;
        }



    }
}
