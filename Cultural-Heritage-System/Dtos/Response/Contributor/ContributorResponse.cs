using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.Contributor
{
    public class ContributorResponse
    {
        public int Id { get; set; }
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public string? DocumentsUrl { get; set; }
        public string Status { get; set; }

        public bool IsPremiumEligible { get; set; }
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public string? CreatedByName { get; set; }
        public string? CreatedByEmail { get; set; }
        public string? UpdatedByName { get; set; }
        public string? UpdatedByEmail { get; set; }
        public int Count { get; set; }

        public string? ExpertiseUnsigned { get; set; }
        public string? FullNameUnsigned { get; set; }

    }
}
