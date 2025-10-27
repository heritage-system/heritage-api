using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class QuizResultDAO : BaseDAO<QuizResult>
    {
        private readonly ILogger<QuizResultDAO> _logger;

        public QuizResultDAO(AppDbContext context, ILogger<QuizResultDAO> logger)
            : base(context)
        {
            _logger = logger;
        }
   
        public async Task<QuizResult?> GetQuizResult(long quizId, int userId)
        {
            var result = await _dbSet
                .FirstOrDefaultAsync(u => u.QuizId == quizId && u.UserId == userId);

            return result;
        }    
    }
}
