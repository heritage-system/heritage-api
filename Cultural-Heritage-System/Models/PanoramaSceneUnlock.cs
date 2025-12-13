using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class PanoramaSceneUnlock : BaseEntity<long>
    {

        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("PanoramaScene")]
        [Column("panorama_scene_id")]
        public long PanoramaSceneId { get; set; }
        public PanoramaScene PanoramaScene { get; set; }

        [Column("unlocking_method", TypeName = "nvarchar(30)")]
        public UnlockingMethod UnlockingMethod { get; set; } = UnlockingMethod.BY_SUBSCRIPTION;


    }

}
