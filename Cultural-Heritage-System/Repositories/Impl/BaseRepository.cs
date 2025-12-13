using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly BaseDAO<T> _dao;

        public BaseRepository(BaseDAO<T> dao)
        {
            _dao = dao;
        }

        public virtual Task<IEnumerable<T>> GetAllAsync()
            => _dao.GetAllAsync();

        public virtual Task<T?> GetByIdAsync(object id)
            => _dao.GetByIdAsync(id);

        public virtual Task AddAsync(T entity)
            => _dao.AddAsync(entity);

        public virtual Task UpdateAsync(T entity)
            => _dao.UpdateAsync(entity);

        public virtual Task DeleteAsync(T entity)
            => _dao.DeleteAsync(entity);

        public Task SaveChangesAsync()
         => _dao.SaveChangesAsync();
    }
}
