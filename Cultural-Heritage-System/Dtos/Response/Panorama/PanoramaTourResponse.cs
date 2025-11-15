using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Panorama
{
    public class PanoramaTourResponse
    {
        public long Id { get; set; }
        public long? HeritageId { get; set; }
        public string Name { get; set; }
        public string? ThumbnailUrl { get; set; }
        public long? DefaultSceneId { get; set; }
        public string? Description { get; set; }
        public PanoramaStatus Status { get; set; }
        public List<PanoramaScene> Scenes { get; set; } = new List<PanoramaScene>();
    }
}
