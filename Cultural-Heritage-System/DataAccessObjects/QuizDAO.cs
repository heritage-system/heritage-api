using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class QuizDAO : BaseDAO<Quiz>
    {
        private readonly ILogger<QuizDAO> _logger;

        public QuizDAO(AppDbContext context, ILogger<QuizDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
        public IQueryable<Quiz> GetQuizQueryable()
        {
            return _dbSet
                .Include(q => q.Questions)
                .Include(q => q.Results)
                .ThenInclude(r => r.User)
                .ThenInclude(u => u.Profile)
                .AsQueryable();
        }

        public async Task<Quiz?> GetQuizById(long id)
        {
            return await _dbSet
               .Include(h => h.Questions)
               .Include(h => h.Results)
               .FirstOrDefaultAsync(u => u.Id == id);
        }
     
    }
}
