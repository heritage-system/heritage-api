using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionAcceptance : BaseEntity<long>
    {
        [Required]
        [Column("contribution_id")]
        [ForeignKey("Contribution")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

        [Required]
        [Column("staff_id")]
        [ForeignKey("Staff")]
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        [Column("accepted_at")]
        public DateTimeOffset AcceptedAt { get; set; } = DateTimeOffset.UtcNow;

      
        [Column("status", TypeName = "nvarchar(20)")]
        public ContributionStatus Status { get; set; } = ContributionStatus.PENDING;

        // Ghi chú của staff khi duyệt
        [Column("note")]
        public string? Note { get; set; }

    }

}
