using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class HeritageLocation: BaseEntity<int>
    {

        [Required]
        [Column("heritage_id")]
        [ForeignKey("Heritage")]
        public long HeritageId { get; set; }
        [JsonIgnore]
        public Heritage Heritage { get; set; }


        [Required]
        [Column("location_id")]
        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public Location Location { get; set; }
    }
}
