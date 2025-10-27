using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class QuizQuestionRepository : BaseRepository<QuizQuestion>, IQuizQuestionRepository
    {
        private readonly QuizQuestionDAO _quizQuestionDAO;

        public QuizQuestionRepository(QuizQuestionDAO QuizQuestionDAO) : base(QuizQuestionDAO)
        {
            _quizQuestionDAO = QuizQuestionDAO;
        }

        public async Task<QuizQuestion?> GetQuizQuestionById(long id)
        {
            return await _quizQuestionDAO.GetQuizQuestionById(id);
        }

        public IQueryable<QuizQuestion> GetQuizQuestionQueryable()
        {
            return _quizQuestionDAO.GetQuizQuestionQueryable();
        }
    }
}
