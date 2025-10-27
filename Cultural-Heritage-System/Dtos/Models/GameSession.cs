using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.GameHubs;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Dtos.Models
{
    public class GameSession
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public List<Player> Players { get; set; } = new();
        public int CurrentQuestionIndex { get; set; }
        public QuizQuestionResponse CurrentQuestion { get; set; } = null!;
        public Dictionary<string, (int answerIndex, double time)> Answers { get; set; } = new();
        public long QuestionStartTime { get; set; }
        public bool IsRevealing { get; set; } = false;
        public List<QuizQuestionResponse> Questions { get; set; } = new();
        public CancellationTokenSource? TimerCts { get; set; }
    }
}
