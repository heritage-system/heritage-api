using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Quiz
{
    public class QuizQuestionCreationRequest
    {     
        public long? QuizId { get; set; }     
        public string Question { get; set; }  
        public string OptionA { get; set; } 
        public string OptionB { get; set; }    
        public string OptionC { get; set; }
        public string OptionD { get; set; }    
        public string CorrectOption { get; set; }
        public QuizCategory? QuizCategory { get; set; }    
        public QuizLevel? QuizLevel { get; set; }
    }
}
