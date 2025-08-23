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
        [Column("occurrence_type", TypeName = "nvarchar(20)")]
        public OccurrenceType OccurrenceType { get; set; }

        [Column("calendar_type", TypeName = "nvarchar(20)")]
        public CalendarType? CalendarType { get; set; }

        [Column("start_day")]
        public int? StartDay { get; set; }

        [Column("start_month")]
        public int? StartMonth { get; set; }

        [Column("end_day")]
        public int? EndDay { get; set; }

        [Column("end_month")]
        public int? EndMonth { get; set; }


        [Column("frequency", TypeName = "nvarchar(20)")]
        public FestivalFrequency? Frequency { get; set; }

        // For Recurring, example: "LastFridayOfOctober"
        [Column("recurrence_rule")]
        public string? RecurrenceRule { get; set; }

        //No information
        [Column("description")]
        public string? Description { get; set; }

        
    }
}
