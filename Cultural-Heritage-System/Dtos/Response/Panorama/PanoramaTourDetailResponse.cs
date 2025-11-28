using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Panorama
{
    public class PanoramaTourDetailResponse
    {
        public long Id { get; set; }
        public long? HeritageId { get; set; }
        public string? HeritageName { get; set; }
        public string Name { get; set; }
        public string? ThumbnailUrl { get; set; }     
        public string? Description { get; set; }
        public PanoramaStatus Status { get; set; }
        public PremiumType PremiumType { get; set; }
        public List<PanoramaSceneResponse> Scenes { get; set; } = new List<PanoramaSceneResponse>();
    }
}
