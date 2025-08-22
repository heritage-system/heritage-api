using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Contributor : BaseEntity<int>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column("bio", TypeName = "text")]
        public string? Bio { get; set; }

        [Column("expertise")]
        public string? Expertise { get; set; }

        [Column("documents_url")]
        public string? DocumentsUrl { get; set; }

        [Column("rating", TypeName = "decimal(3,2)")]
        public decimal? Rating { get; set; }   // VD: 4.75

        [Column("verified")]
        public bool Verified { get; set; } = false;

        [Column("status", TypeName = "nvarchar(20)")]
        public ContributorStatus Status { get; set; } = ContributorStatus.APPLIED;
      
        public ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();
       
    }
}
