using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class CategoryDAO : BaseDAO<Category>
    {
        private readonly ILogger<CategoryDAO> _logger;

        public CategoryDAO(AppDbContext context, ILogger<CategoryDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<Category> GetCategoriesQueryable()
        {
            return _context.Categories.AsQueryable();
        }

    }
}
