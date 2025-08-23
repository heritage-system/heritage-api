using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Location: BaseEntity<int>
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("code")]
        public string Code { get; set; }  //HN, HCM, DN...
        public ICollection<HeritageLocation> HeritageLocations { get; set; } = new List<HeritageLocation>();
    }
}
