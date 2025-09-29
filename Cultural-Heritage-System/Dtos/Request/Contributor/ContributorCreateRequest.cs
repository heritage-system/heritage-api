namespace Cultural_Heritage_System.Dtos.Request.Contributor
{
    public class ContributorCreateRequest
    {
        public int UserId { get; set; }
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public bool IsPremiumEligible { get; set; } = false;
    }
}
