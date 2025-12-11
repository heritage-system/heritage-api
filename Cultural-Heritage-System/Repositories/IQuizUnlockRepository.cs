using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IQuizUnlockRepository : IBaseRepository<QuizUnlock>
    {
        Task<QuizUnlock> GetQuizUnlockByUserAndQuiz(int userId, long quizId);
        IQueryable<QuizUnlock> GetQuizUnlocksQueryByUserId(int userId);
        Task<bool> IsQuizUnlockExists(int userId, long quizId);
        Task<int> GetQuizUnlockCountByUserId(int userId);
        
    }
}
