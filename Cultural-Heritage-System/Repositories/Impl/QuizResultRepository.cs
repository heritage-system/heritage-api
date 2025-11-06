using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class QuizResultRepository : BaseRepository<QuizResult>, IQuizResultRepository
    {
        private readonly QuizResultDAO _entityDAO;

        public QuizResultRepository(QuizResultDAO QuizDAO) : base(QuizDAO)
        {
            _entityDAO = QuizDAO;
        }
     
        public Task<QuizResult?> GetQuizResult(long quizId, int userId)
        {
            return _entityDAO.GetQuizResult(quizId, userId);
        }

        public Task<List<QuizResult>> GetResultsByQuizId(long quizId)
        {
            return _entityDAO.GetResultsByQuizId(quizId);
        }
    }
}
