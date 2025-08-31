using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ReviewMedia : BaseEntity<long>
    {
        [Required]
        [ForeignKey("Review")]
        [Column("review_id")]
        public long ReviewId { get; set; }
        public Review Review { get; set; }

        [Column("media_type", TypeName = "nvarchar(20)")]
        public MediaType MediaType { get; set; }

        [Required]
        [Column("url")]
        public string Url { get; set; }
       
    }

}
