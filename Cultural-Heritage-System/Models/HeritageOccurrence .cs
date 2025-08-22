using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class HeritageOccurrence: BaseEntity<long>
    {

        [Required]
        [Column("heritage_id")]
        [ForeignKey("Heritage")]
        public long HeritageId { get; set; }
        public Heritage Heritage { get; set; }

        //Occurrence Type(ExactDate, Range, RecurringRule, Approximate, Unknown)
        [Column("occurrence_type")]
        public OccurrenceType OccurrenceType { get; set; }

        [Column("calendar_type")]
        public CalendarType? CalendarType { get; set; }

        [Column("start_date")]
        public DateTime? StartDate { get; set; }

        [Column("end_date")]
        public DateTime? EndDate { get; set; }

        [Column("frequency")]
        public FestivalFrequency? Frequency { get; set; }

        // For Recurring, example: "LastFridayOfOctober"
        [Column("recurrence_rule")]
        public string RecurrenceRule { get; set; }

        //No information
        [Column("description")]
        public string Description { get; set; }

        
    }
}
