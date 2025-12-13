using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.DataAccessObjects
{
    public class QuizUnlockDAO : BaseDAO<QuizUnlock>
    {
        private readonly ILogger<QuizUnlockDAO> _logger;

        public QuizUnlockDAO(AppDbContext context, ILogger<QuizUnlockDAO> logger)
            : base(context)
        {
            _logger = logger;
        }

        public async Task<QuizUnlock> GetQuizUnlockByUserAndQuiz(int userId, long quizId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.UserId == userId && f.QuizId == quizId);
        }

  


        public IQueryable<QuizUnlock> GetQuizUnlocksQueryByUserId(int userId)
        {
            return _dbSet
                .Include(f => f.Quiz)            
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt);
        }

        public async Task<bool> IsQuizUnlockExists(int userId, long quizId)
        {
            return await _dbSet
                .AnyAsync(f => f.UserId == userId && f.QuizId == quizId);
        }

        public async Task<int> GetQuizUnlockCountByUserId(int userId)
        {
            return await _dbSet
                .CountAsync(f => f.UserId == userId);
        }
    }
}
