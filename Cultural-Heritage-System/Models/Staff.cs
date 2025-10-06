using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Staff : BaseEntity<int>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("staff_role", TypeName = "nvarchar(50)")]
        public StaffRole StaffRole { get; set; } = StaffRole.CONTENT_REVIEWER;

       
        [Column("staff_status", TypeName = "nvarchar(20)")]
        public StaffStatus StaffStatus { get; set; } = StaffStatus.ACTIVE;

        // Ngày bắt đầu công việc
        [Column("start_date")]
        public DateTimeOffset StartDate { get; set; } = DateTime.UtcNow;

       
        [Column("end_date")]
        public DateTimeOffset? EndDate { get; set; }

        // Staff có quyền gì ngoài CRUD Heritage & Duyệt bài

        [Column("can_manage_events")]
        public bool CanManageEvents { get; set; } = false;

        [Column("can_reply_reports")]
        public bool CanReplyReports { get; set; } = false;

        [Column("can_assign_tasks")]
        public bool CanAssignTasks { get; set; } = false;

        public ICollection<Heritage> ManagedHeritages { get; set; } = new List<Heritage>();

        public ICollection<ContributionAcceptance> ContributionAcceptances { get; set; } = new List<ContributionAcceptance>();

    }
}
