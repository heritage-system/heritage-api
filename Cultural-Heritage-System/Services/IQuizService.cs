
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Models;


namespace Cultural_Heritage_System.Services
{
    public interface IQuizService
    {
        Task<List<QuizQuestionResponse>> GenerateQuestionSet(int numberOfQuestions);
    }
}
