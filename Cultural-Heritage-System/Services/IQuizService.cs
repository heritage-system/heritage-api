
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Quiz;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Quiz;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Models;


namespace Cultural_Heritage_System.Services
{
    public interface IQuizService
    {
        Task<List<QuizQuestionResponse>> GenerateQuestionSet(int numberOfQuestions);
        Task<QuizDetailResponse> GetQuizDetail(long quizId);
        Task<PageResponse<QuizListResponse>> GetListQuiz(QuizListRequest request);
        Task<bool> SaveQuizResult(SaveQuizResultRequest request);
        Task<bool> CreateQuiz(QuizCreationRequest request);
        Task<bool> UpdateQuiz(QuizUpdateRequest request);
        Task<long?> DeleteQuiz(long id);
        Task<bool> CreateQuizQuestion(QuizQuestionCreationRequest request);
        Task<bool> UpdateQuizQuestion(QuizQuestionUpdateRequest request);
        Task<long?> DeleteQuizQuestion(long id);
        Task<QuizDetailAdminResponse> GetQuizDetailAdmin(long quizId);
        Task<QuizOverviewResponse> GetQuizOverview(long quizId);
        Task<bool> UnlockQuiz(long quizId);
    }
}
