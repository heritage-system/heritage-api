using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Contributor
{
    public class ContributorUpdateRequest
    {
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public string? Status { get; set; }
    }
}
