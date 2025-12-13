using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories
{
    public interface IQuizResultRepository : IBaseRepository<QuizResult>
    {
        Task<QuizResult?> GetQuizResult(long quizId, int userId);
        Task<List<QuizResult>> GetResultsByQuizId(long quizId);
    }
}
