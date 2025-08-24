using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class HeritageOccurrenceDto 
    {
        public long Id { get; set; }            
        public string OccurrenceTypeName { get; set; }
        public string? CalendarTypeName { get; set; }
        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }     
        public string? FrequencyName { get; set; }
        public string? RecurrenceRule { get; set; }
        public string? Description { get; set; }

        
    }
}
