using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PanoramaTour : BaseEntity<long>
    {
     
        [Column("heritage_id")]
        [ForeignKey("Heritage")]
        public long? HeritageId { get; set; }
        public Heritage? Heritage { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("thumbnail_url")]
        public string? ThumbnailUrl { get; set; }

        [Column("default_scene_id")]
        public long? DefaultSceneId { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("status", TypeName = "nvarchar(20)")]
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;

        [Column("premium_type", TypeName = "nvarchar(30)")]
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;

        public ICollection<PanoramaScene> Scenes { get; set; } = new List<PanoramaScene>();
    }
}
 