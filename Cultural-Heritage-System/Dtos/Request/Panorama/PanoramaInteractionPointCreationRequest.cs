using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Panorama
{
    public class PanoramaInteractionPointCreationRequest
    {   
        public long? PanoramaSceneId { get; set; }    
        public string Label { get; set; }
        public InteractionType Type { get; set; }
        public string? TargetSceneName { get; set; } // for MoveToScene
        public string? AudioUrl { get; set; }        // for PlayAudio
        public string? NoteText { get; set; }        // for ShowNote
        public string? LinkUrl { get; set; }         // for OpenLink
        // Visuals & transform    
        public float PositionX { get; set; }      
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public float RotationY { get; set; } = 0f;
        public string? IconUrl { get; set; }
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;
    }
}
