namespace Cultural_Heritage_System.Dtos.Response
{
    public class OccurrenceResponse
    {
        public long Id { get; set; }

        public string OccurrenceType { get; set; } 
        public string? CalendarType { get; set; } 
        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }
        public string? Frequency { get; set; } 
        public string? RecurrenceRule { get; set; }
        public string? Description { get; set; }
    }
}
