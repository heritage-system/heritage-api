using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class Tag : BaseEntity<int>
    {
        [Column("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<HeritageTag> HeritageTags { get; set; } = new List<HeritageTag>();
    }

}
