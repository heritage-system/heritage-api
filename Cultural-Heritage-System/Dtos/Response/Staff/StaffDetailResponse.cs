using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Staff
{
    public class StaffDetailResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public StaffStatus StaffStatus { get; set; }
        public StaffRole StaffRole { get; set; }      
        public DateTimeOffset StartDate { get; set; } 
        public bool CanManageEvents { get; set; } 
        public bool CanReplyReports { get; set; }
        public bool CanAssignTasks { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
      
        //static
        public int NumberOfContributionAcceptances { get; set; }
        public int NumberOfAcceptedContributions { get; set; }
        public int NumberOfDeniedContributions { get; set; }
        public int NumberOfReportReplies { get; set; }       
    }
}
