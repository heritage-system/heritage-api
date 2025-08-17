using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class QuizQuestion : BaseEntity<long>
    {

        [Column("quiz_id")]
        [ForeignKey("Quiz")]
        public long QuizId { get; set; }
        public Quiz Quiz { get; set; }


        [Column("question")]
        public string Question { get; set; }

        [Column("option_a")]
        public string OptionA { get; set; }

        [Column("option_b")]
        public string OptionB { get; set; }

        [Column("option_c")]
        public string OptionC { get; set; }

        [Column("option_d")]
        public string OptionD { get; set; }

        [Column("correct_option")]
        public string CorrectOption { get; set; }
      
        [Required]
        [Column("quiz_category_id")]
        [ForeignKey("QuizCategory")]    
        public int QuizCategoryId { get; set; }
        public QuizCategory QuizCategory { get; set; }


        [Required]
        [ForeignKey("QuizRank")]
        [Column("quiz_rank_id")]
        public int QuizRankId { get; set; }
        public QuizRank? QuizRank { get; set; }


    }

}
