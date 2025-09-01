using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response
{
    public class ContributorResponse
    {
        public int Id { get; set; }
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public string? DocumentsUrl { get; set; }
        public decimal? Rating { get; set; }
        public bool Verified { get; set; }
        public ContributorStatus Status { get; set; }

        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserEmail { get; set; }
    }
}
