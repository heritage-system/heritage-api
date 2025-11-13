using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Panorama
{
    public class PanoramaSceneCreationRequest
    {
        public long? PanoramaTourId { get; set; }    
        public string SceneName { get; set; }
        public string PanoramaUrl { get; set; }
        public string? AmbientSoundUrl { get; set; }
        public string? Description { get; set; }
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;
        public List<PanoramaInteractionPointCreationRequest> InteractionPoints { get; set; } = new List<PanoramaInteractionPointCreationRequest>();
    }
}
