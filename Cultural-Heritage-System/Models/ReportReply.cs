using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class ReportReply : BaseEntity<long>
    {
        [Required]
        [ForeignKey("Report")]
        [Column("report_id")]
        public long ReportId { get; set; }
        public Report Report { get; set; }
        [Required]
        [Column("message")]
        public string Message { get; set; }
    }

}
