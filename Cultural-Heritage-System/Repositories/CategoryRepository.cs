using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class CategoryRepository : BaseRepository<Category>
    {
        private readonly ILogger<CategoryRepository> _logger;

        public CategoryRepository(AppDbContext context, ILogger<CategoryRepository> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<Category> GetCategoriesQueryable()
        {
            return _context.Categories.Include(h => h.Heritages).AsQueryable();
        }

    }
}
