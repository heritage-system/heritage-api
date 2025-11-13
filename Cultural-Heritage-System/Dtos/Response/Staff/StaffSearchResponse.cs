using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.Staff
{
    public class StaffSearchResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public StaffStatus StaffStatus { get; set; }
        public StaffRole StaffRole { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
