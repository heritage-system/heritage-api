namespace Cultural_Heritage_System.Dtos.Response.Tag
{
    public class TagSearchResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameUnsigned { get; set; }

        public string CreatedBy { get; set; }
        public string? CreateByName { get; set; }

        public string? CreateByEmail { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public string? UpdatedByEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Count of linked heritages
        public int Count { get; set; }
    }

}
