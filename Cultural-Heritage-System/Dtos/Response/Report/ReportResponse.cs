namespace Cultural_Heritage_System.Dtos.Response.Report
{
    public class ReportResponse
    {
        public long Id { get; set; }
        public int UserId { get; set; } 
        public long HeritageId { get; set; }
        public string HeritageName { get; set; }
        public string Reason { get; set; }
        public string? CreatedAt { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
    }
}


