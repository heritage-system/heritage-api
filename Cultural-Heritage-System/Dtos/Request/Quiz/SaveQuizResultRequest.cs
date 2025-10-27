using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class SaveQuizResultRequest
    {
        public long QuizId { get; set; }    
        public int NumberOfClear { get; set;}   
    }
}
