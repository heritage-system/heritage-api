namespace Cultural_Heritage_System.Dtos.Request.Report
{
    public class CreateReportRequest
    {
        public int UserId { get; set; }
        public long HeritageId { get; set; }
        public string Reason { get; set; }
    }
}


