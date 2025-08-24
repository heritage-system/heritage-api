namespace Cultural_Heritage_System.Dtos.Response
{
    public class HeritageLocationResponse
    {
        public long HeritageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<CoordinateDto> Coordinates { get; set; } = new();
        public List<LocationDto> Locations { get; set; } = new();
    }

    public class CoordinateDto
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

    public class LocationDto
    {
        public int LocationId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
