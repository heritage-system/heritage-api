using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class ContributionReportReply : BaseEntity<long>
    {
        [Required]
        [ForeignKey("ContributionReport")]
        [Column("contribution_report_id")]
        public long ContributionReportId { get; set; }
        public ContributionReport ContributionReport { get; set; }
        [Required]
        [Column("message")]
        public string Message { get; set; }

        [ForeignKey("Staff")]
        [Column("staff_id")]
        public int StaffId { get; set; }
        public Staff Staff { get; set; }
    }

}
