using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Panorama
{
    public class PanoramaSceneResponse
    {
        public long Id { get; set; }
        public long? PanoramaTourId { get; set; }
        public string SceneName { get; set; }
        public string SceneThumbnail { get; set; }
        public string? PanoramaUrl { get; set; }
        public string? Description { get; set; }
        public PanoramaStatus Status { get; set; }
        public PremiumType PremiumType { get; set; }
    }
}
