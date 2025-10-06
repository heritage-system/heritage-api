using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO) : base(categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }


        public IQueryable<Category> GetCategoriesQueryable()
        {
            return _categoryDAO.GetCategoriesQueryable();
        }

    }
}
