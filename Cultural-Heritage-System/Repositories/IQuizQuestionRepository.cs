using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IQuizQuestionRepository : IBaseRepository<QuizQuestion>
    {
        IQueryable<QuizQuestion> GetQuizQuestionQueryable();
        Task<QuizQuestion?> GetQuizQuestionById(long id);


    }
}
