using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Quiz
{
    public class QuizListResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }    
        public string BannerUrl { get; set; }
        public int TotalQuestions { get; set; }
        public int NumberOfClear { get; set; } = 0;
        public bool isPremium { get; set; } = false;
    }
}
