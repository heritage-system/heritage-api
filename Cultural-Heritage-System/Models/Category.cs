using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class Category: BaseEntity<int>
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [JsonIgnore]
        public ICollection<Heritage> Heritages { get; set; } = new List<Heritage>();
    }

}
