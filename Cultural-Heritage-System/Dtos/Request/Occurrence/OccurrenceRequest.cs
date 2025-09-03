using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request.Occurrence
{
    public class OccurrenceRequest
    {
        [Required]
        public string OccurrenceType { get; set; } // ExactDate, Range, RecurringRule, Approximate, Unknown

        public string? CalendarType { get; set; } // Gregorian, Lunar, etc.

        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }

        public string? Frequency { get; set; } // Yearly, Monthly, etc.

        public string? RecurrenceRule { get; set; } // e.g. "LastFridayOfOctober"

        public string? Description { get; set; }
    }

}
