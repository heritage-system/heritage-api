using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Quiz : BaseEntity<long>
    {

        [Column("title")]
        public string Title { get; set; }

        [Required]
        [Column("created_by")]
        [ForeignKey("User")]      
        public int CreatedBy { get; set; }
        public User User { get; set; }

        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public ICollection<QuizResult> Results { get; set; } = new List<QuizResult>();
    }

}
