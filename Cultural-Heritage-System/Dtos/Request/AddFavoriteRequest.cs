using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class AddFavoriteRequest
    {
        [Required]
        public long HeritageId { get; set; }
    }
}
