using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PanoramaScene : BaseEntity<long>
    {
     
        [Column("panorama_tour_id")]
        [ForeignKey("PanoramaTour")]
        public long? PanoramaTourId { get; set; }
        public PanoramaTour? PanoramaTour { get; set; }

        [Column("scene_name")]
        public string SceneName { get; set; }

        [Column("panorama_url")]
        public string PanoramaUrl { get; set; }

        [Column("ambient_sound_url")]
        public string? AmbientSoundUrl { get; set; }
       
        [Column("description")]
        public string? Description { get; set; }

        [Column("status", TypeName = "nvarchar(20)")]
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;

        public ICollection<PanoramaInteractionPoint> InteractionPoints { get; set; } = new List<PanoramaInteractionPoint>();
    }
}
