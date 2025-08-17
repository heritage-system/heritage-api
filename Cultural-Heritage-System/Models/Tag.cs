using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Tag: BaseEntity<int>
    {
        [Column("name")]
        public string Name { get; set; }

        public ICollection<HeritageTag> HeritageTags { get; set; } = new List<HeritageTag>();
    }

}
