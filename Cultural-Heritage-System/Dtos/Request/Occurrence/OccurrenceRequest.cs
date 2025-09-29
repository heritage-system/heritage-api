using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Occurrence
{
    public class OccurrenceRequest
    {      
        public OccurrenceType OccurrenceType { get; set; }
 
        public CalendarType? CalendarType { get; set; }
  
        public int? StartDay { get; set; }

        public int? StartMonth { get; set; }
    
        public int? EndDay { get; set; }       
        public int? EndMonth { get; set; }  
        public FestivalFrequency? Frequency { get; set; }     
        public string? RecurrenceRule { get; set; }     
        public string? Description { get; set; }

    }

}
