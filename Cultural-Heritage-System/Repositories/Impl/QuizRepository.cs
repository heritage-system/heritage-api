using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class QuizRepository : BaseRepository<Quiz>, IQuizRepository
    {
        private readonly QuizDAO _quizDAO;

        public QuizRepository(QuizDAO QuizDAO) : base(QuizDAO)
        {
            _quizDAO = QuizDAO;
        }

        public Task<Quiz?> GetQuizById(long id)
        {
            return _quizDAO.GetQuizById(id);
        }

        public IQueryable<Quiz> GetQuizQueryable()
        {
            return _quizDAO.GetQuizQueryable();
        }
     
    }
}
