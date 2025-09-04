namespace Cultural_Heritage_System.Dtos.Request.Contributor
{
    public class ContributorCreateRequest
    {
        public string UserEmail { get; set; }
        public string? Bio { get; set; }
        public string? Expertise { get; set; }
        public string? DocumentsUrl { get; set; }
    }
}
