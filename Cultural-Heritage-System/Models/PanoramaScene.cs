using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PanoramaScene : BaseEntity<long>, IUnsignedEntity
    {
     
        [Column("panorama_tour_id")]
        [ForeignKey("PanoramaTour")]
        public long? PanoramaTourId { get; set; }
        public PanoramaTour? PanoramaTour { get; set; }

        [Column("scene_name")]
        public string SceneName { get; set; }

        [Column("scene_thumbnail")]
        public string SceneThumbnail { get; set; }

        [Column("panorama_url")]
        public string PanoramaUrl { get; set; }  
       
        [Column("description")]
        public string? Description { get; set; }

        [Column("status", TypeName = "nvarchar(20)")]
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;

        [Column("premium_type", TypeName = "nvarchar(30)")]
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;

        [Column("scene_name_unsigned")]
        public string SceneNameUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            SceneNameUnsigned = StringHelper.RemoveDiacritics(SceneName).ToLower();
        }
    }
}
