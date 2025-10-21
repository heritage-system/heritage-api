using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.QuizQuestion
{
    public class QuizQuestionResponse
    {    
        public long Id { get; set; }
        public long? QuizId { get; set; }     
        public string Question { get; set; }     
        public string OptionA { get; set; }     
        public string OptionB { get; set; }    
        public string OptionC { get; set; }  
        public string OptionD { get; set; }
        public string CorrectOption { get; set; }
        public QuizCategory? QuizCategory { get; set; }
        public QuizLevel? QuizLevel { get; set; }

        public string[] ToOptionsArray()
        {
            var options = new List<string>();

            if (!string.IsNullOrWhiteSpace(OptionA)) options.Add(OptionA);
            if (!string.IsNullOrWhiteSpace(OptionB)) options.Add(OptionB);
            if (!string.IsNullOrWhiteSpace(OptionC)) options.Add(OptionC);
            if (!string.IsNullOrWhiteSpace(OptionD)) options.Add(OptionD);

            return options.ToArray();
        }
    }
}
