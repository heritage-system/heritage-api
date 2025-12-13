namespace Cultural_Heritage_System.Dtos.Response.Location
{
    public class LocationResponse
    {
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? AddressDetail { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
