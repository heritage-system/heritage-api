using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        public IQueryable<Category> GetCategoriesQueryable();      
    }
}
