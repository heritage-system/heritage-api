using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Panorama
{
    public class PanoramaTourSearchForAdminResponse
    {
        public long Id { get; set; }
        public long? HeritageId { get; set; }
        public string? HeritageName { get; set; }       
        public string Name { get; set; }    
        public PanoramaStatus Status { get; set; }
        public PremiumType PremiumType { get; set; }     
        public int NumberOfScenes { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
