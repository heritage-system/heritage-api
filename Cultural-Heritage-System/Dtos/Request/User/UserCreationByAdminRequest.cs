using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class UserCreationByAdminRequest
    {     
        public string Username { get; set; }     
        public string Email { get; set; }   
        public string FullName { get; set; }
        public string RoleName { get; set; } = DefinitionRole.MEMBER;

        //Staff
        public StaffRole StaffRole { get; set; } = StaffRole.CONTENT_REVIEWER;
        public bool CanManageEvents { get; set; } = false;
        public bool CanReplyReports { get; set; } = false;
        public bool CanAssignTasks { get; set; } = false;

        //Contributor
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public bool IsPremiumEligible { get; set; } = false;
    }
}
