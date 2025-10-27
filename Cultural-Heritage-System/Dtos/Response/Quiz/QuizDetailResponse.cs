using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Quiz
{
    public class QuizDetailResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }   
        public int NumberOfClear { get; set; }
        public List<QuizQuestionResponse> Questions { get; set; } = new List<QuizQuestionResponse>(); 
        
    }
}
