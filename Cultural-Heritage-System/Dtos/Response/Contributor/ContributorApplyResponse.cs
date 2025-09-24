using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.Contributor
{
    public class ContributorApplyResponse
    {
        public int Id { get; set; }
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public string? DocumentsUrl { get; set; }
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
