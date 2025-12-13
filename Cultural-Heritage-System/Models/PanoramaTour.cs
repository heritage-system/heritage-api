using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PanoramaTour : BaseEntity<long>, IUnsignedEntity
    {
     
        [Column("heritage_id")]
        [ForeignKey("Heritage")]
        public long? HeritageId { get; set; }
        public Heritage? Heritage { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }       

        [Column("description")]
        public string? Description { get; set; }

        [Column("status", TypeName = "nvarchar(20)")]
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;

        [Column("premium_type", TypeName = "nvarchar(30)")]
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;

        public ICollection<PanoramaScene> Scenes { get; set; } = new List<PanoramaScene>();

        [Column("name_unsigned")]
        public string NameUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            NameUnsigned = StringHelper.RemoveDiacritics(Name).ToLower();
        }
    }
}
 