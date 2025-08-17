using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class HeritageTag : BaseEntity<int>
    {

        [Required]
        [Column("heritage_id")]
        [ForeignKey("Heritage")]
        public long HeritageId { get; set; }
        public Heritage Heritage { get; set; }


        [Required]
        [Column("tag_id")]
        [ForeignKey("Tag")]
        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

}