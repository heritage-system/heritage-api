namespace Cultural_Heritage_System.Dtos.Response.ContributionReport
{
    public class ContributionReportResponse
    {
        public long Id { get; set; }
        public int UserId { get; set; } 
        public long ContributionId { get; set; }
        public string ContributionName { get; set; }
        public string Reason { get; set; }
        public string? CreatedAt { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
    }
}


