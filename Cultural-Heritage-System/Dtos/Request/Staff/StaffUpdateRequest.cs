using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Staff
{
    public class StaffUpdateRequest
    {    
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }    
        public StaffStatus? StaffStatus { get; set; }
        public StaffRole? StaffRole { get; set; }  
        public bool? CanManageEvents { get; set; }
        public bool? CanReplyReports { get; set; }
        public bool? CanAssignTasks { get; set; }
    }
}
