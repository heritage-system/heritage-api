using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;

namespace Cultural_Heritage_System.Dtos.Response.Quiz
{
    public class QuizDetailAdminResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string BannerUrl { get; set; }
        public PremiumType PremiumType { get; set; }
        public int NumberOfClear { get; set; }

        public int TotalQuestions { get; set; }
        public int TotalAttempts { get; set; }
        public int TotalClearCount { get; set; }
        public List<QuizResultInfo> Results { get; set; } = new();
        public List<QuizQuestionResponse> Questions { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class QuizResultInfo
    {
        public int UserId { get; set; }
        public int NumberOfClear { get; set; }
    }
}
