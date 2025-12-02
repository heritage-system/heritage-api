using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Contributor
{
    public class ContributorUpdateRequest
    {
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public string? Status { get; set; }
        public string? DocumentsUrl { get; set; }
        public bool? IsPremiumEligible { get; set; }
    }
}
