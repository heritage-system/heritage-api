using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Contributor
{
    public class ContributorUpdateRequest
    {
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public bool Verified { get; set; }
        public ContributorStatus Status { get; set; }
    }
}
