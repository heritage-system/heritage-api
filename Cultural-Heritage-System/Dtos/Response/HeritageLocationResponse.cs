using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Dtos.Response
{
    public class HeritageLocationResponse
    {
        public long HeritageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<HeritageCoordinateDto> Coordinates { get; set; } = new();
        public List<HeritageLocationDto> Locations { get; set; } = new();
    }

}
