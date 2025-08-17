using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Heritage : BaseEntity<long>
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("category_id")]
        [ForeignKey("Category")]    
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Column("map_url")]
        public string MapUrl { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("is_featured")]
        public bool IsFeatured { get; set; }

        public ICollection<HeritageTag> HeritageTags { get; set; } = new List<HeritageTag>();
   
        public ICollection<HeritageLocation> HeritageLocations { get; set; } = new List<HeritageLocation>();
    }

}
