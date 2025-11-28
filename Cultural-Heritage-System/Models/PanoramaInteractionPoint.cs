//using Cultural_Heritage_System.Common;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace Cultural_Heritage_System.Models
//{
//    public class PanoramaInteractionPoint : BaseEntity<long>
//    {
//        [Column("panorama_scene_id")]
//        [ForeignKey("PanoramaScene")]
//        public long? PanoramaSceneId { get; set; }
//        public PanoramaScene? PanoramaScene { get; set; }

//        [Column("label")]
//        public string Label { get; set; }

//        [Column("type", TypeName = "nvarchar(20)")]
//        public InteractionType Type { get; set; }


//        // Conditional fields
//        [Column("target_scene_id")]
//        public long? TargetSceneId { get; set; } // for MoveToScene

//        [Column("audio_url")]
//        public string? AudioUrl { get; set; }        // for PlayAudio

//        [Column("note_text")]
//        public string? NoteText { get; set; }        // for ShowNote

//        [Column("link_url")]
//        public string? LinkUrl { get; set; }         // for OpenLink

//        // Visuals & transform
//        [Column("position_x")]
//        public float PositionX { get; set; }

//        [Column("position_y")]
//        public float PositionY { get; set; }

//        [Column("position_z")]
//        public float PositionZ { get; set; }

//        [Column("rotation_y")]
//        public float RotationY { get; set; } = 0f;

//        [Column("icon_url")]
//        public string? IconUrl { get; set; }

//        [Column("status", TypeName = "nvarchar(20)")]
//        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;
//    }
//}
