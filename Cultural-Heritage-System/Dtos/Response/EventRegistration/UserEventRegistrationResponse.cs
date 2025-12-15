using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.EventRegistration
{
    public class UserEventRegistrationResponse
    {       
        public long Id { get; set; }
        public long EventId { get; set; }
        public string Title { get; set; }
        public string? ThumbnailUrl { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime? CloseAt { get; set; }
    
        public EventCategory Category { get; set; }

        public EventTag Tags { get; set; }

        public int UserId { get; set; }
      
        public DateTime RegisteredAt { get; set; }
              
    }
}
