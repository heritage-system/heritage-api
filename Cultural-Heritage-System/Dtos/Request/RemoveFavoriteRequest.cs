using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class RemoveFavoriteRequest
    {
        [Required]
        public long HeritageId {  get; set; }   
    }
}
