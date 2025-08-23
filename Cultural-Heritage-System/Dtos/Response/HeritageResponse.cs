namespace Cultural_Heritage_System.Dtos.Response
{
    public class HeritageResponse
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string MapUrl { get; set; }

        public bool IsFeatured { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<MediaResponse> Media { get; set; } = new List<MediaResponse>();
        public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
        public List<LocationResponse> Locations { get; set; } = new List<LocationResponse>();
        public List<OccurrenceResponse> Occurrences { get; set; } = new List<OccurrenceResponse>();
        public List<CoordinateResponse> Coordinates { get; set; } = new List<CoordinateResponse>();
    }

}
