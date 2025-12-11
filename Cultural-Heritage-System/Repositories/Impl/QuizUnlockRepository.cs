using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class QuizUnlockRepository : BaseRepository<QuizUnlock>, IQuizUnlockRepository
    {
        private readonly QuizUnlockDAO _entityDAO;

        public QuizUnlockRepository(QuizUnlockDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<QuizUnlock> GetQuizUnlockByUserAndQuiz(int userId, long quizId)
        {
            return await _entityDAO.GetQuizUnlockByUserAndQuiz(userId,quizId);
        }

 

        public IQueryable<QuizUnlock> GetQuizUnlocksQueryByUserId(int userId)
        {
            return _entityDAO.GetQuizUnlocksQueryByUserId(userId);
        }

        public async Task<bool> IsQuizUnlockExists(int userId, long quizId)
        {
            return await _entityDAO.IsQuizUnlockExists(userId,quizId);
        }

        public async Task<int> GetQuizUnlockCountByUserId(int userId)
        {
            return await _entityDAO.GetQuizUnlockCountByUserId(userId);
        }
    }
}
