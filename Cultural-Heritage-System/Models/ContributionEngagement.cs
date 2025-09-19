using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class ContributionEngagement: BaseEntity<long>
    {
        [Required]
        [ForeignKey("User")]
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        [ForeignKey("Contribution")]
        [Column("contribution_id")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

        [Column("seconds_read")]
        public int SecondsRead { get; set; }
        
        [Column("percent_completed")]
        public double PercentCompleted { get; set; }
        [Required]
        [Column("captured_at")]
        public DateTime CapturedAt { get; set; }
    
    }


}
