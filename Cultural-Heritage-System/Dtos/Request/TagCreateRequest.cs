using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class TagCreateRequest
    {
        [Required]
        public string Name { get; set; }
    }

}
