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

        public IQueryable<QuizQuestion> GetQuizQuestionQueryable()
        {
            return _context.QuizQuestions.AsQueryable();
        }

    }
}
