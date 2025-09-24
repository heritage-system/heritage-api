using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionSave: BaseEntity<long>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [Column("contribution_id")]
        [ForeignKey("Contribution")]      
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

    }

}
