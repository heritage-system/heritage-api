using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class HeritageMedia : BaseEntity<long>
    {
        [Required]
        [ForeignKey("Heritage")]
        [Column("heritage_id")]
        public long HeritageId { get; set; }
        public Heritage Heritage { get; set; }

        [Column("media_type", TypeName = "nvarchar(20)")]
        public MediaType MediaType { get; set; }

        [Required]
        [Column("url")]
        public string Url { get; set; }

        [Column("description")]
        public string? Description { get; set; }

       
    }

}
