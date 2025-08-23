using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;

namespace Cultural_Heritage_System.Models
{
    public class Heritage : BaseEntity<long>,IUnsignedEntity
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

        [Column("is_featured")]
        public bool IsFeatured { get; set; }


        public ICollection<HeritageOccurrence> HeritageOccurrences { get; set; } = new List<HeritageOccurrence>();

        public ICollection<HeritageMedia> Media { get; set; } = new List<HeritageMedia>();

        public ICollection<HeritageTag> HeritageTags { get; set; } = new List<HeritageTag>();
   
        public ICollection<HeritageLocation> HeritageLocations { get; set; } = new List<HeritageLocation>();

        public ICollection<HeritageCoordinate> Coordinates { get; set; } = new List<HeritageCoordinate>();


        [Column("name_unsigned")]
        public string NameUnsigned { get; set; }

        [Column("description_unsigned")]
        public string DescriptionUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            NameUnsigned = StringHelper.RemoveDiacritics(Name).ToLower();
            DescriptionUnsigned = StringHelper.RemoveDiacritics(Description).ToLower();
        }
    }

}
