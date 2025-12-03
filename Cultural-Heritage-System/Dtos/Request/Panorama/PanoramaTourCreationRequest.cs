using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Panorama
{
    public class PanoramaTourCreationRequest
    {      
        public long? HeritageId { get; set; }       
        public string Name { get; set; }
        public string? ThumbnailUrl { get; set; }
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;
        public string? Description { get; set; }
        public PanoramaStatus Status { get; set; } = PanoramaStatus.ACTIVE;
        public List<PanoramaSceneCreationRequest> Scenes { get; set; } = new List<PanoramaSceneCreationRequest>();
    }
}
