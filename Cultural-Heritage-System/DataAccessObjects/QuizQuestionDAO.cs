using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class QuizQuestionDAO : BaseDAO<QuizQuestion>
    {
        private readonly ILogger<QuizQuestionDAO> _logger;

        public QuizQuestionDAO(AppDbContext context, ILogger<QuizQuestionDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public IQueryable<QuizQuestion> GetQuizQuestionQueryable()
        {
            return _context.QuizQuestions.Include(q => q.Quiz).AsQueryable();
        }

        public async Task<QuizQuestion?> GetQuizQuestionById(long id)
        {
            return await _dbSet             
               .FirstOrDefaultAsync(u => u.Id == id);
        }

    }
}
