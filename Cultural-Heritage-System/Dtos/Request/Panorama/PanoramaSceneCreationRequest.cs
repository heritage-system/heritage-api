using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Panorama
{
    public class PanoramaSceneCreationRequest
    {
        public long? PanoramaTourId { get; set; }    
        public string SceneName { get; set; }
        public string SceneThumbnail { get; set; }
        public string PanoramaUrl { get; set; }       
        public string? Description { get; set; }
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;
    }
}
