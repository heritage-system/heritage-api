using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Models
{

    public class QuizRank : BaseEntity<int>
    {
        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        public virtual ICollection<QuizQuestion> Users { get; set; } = new List<QuizQuestion>();
    }
}
