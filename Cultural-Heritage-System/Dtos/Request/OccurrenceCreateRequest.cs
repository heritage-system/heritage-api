using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class OccurrenceCreateRequest
    {
        [Required]
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

}
