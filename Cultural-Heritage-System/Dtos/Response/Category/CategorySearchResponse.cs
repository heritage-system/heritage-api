namespace Cultural_Heritage_System.Dtos.Response.Category
{
    public class CategorySearchResponse
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }

        // New fields
        public string NameUnsigned { get; set; }

        public string DescriptionUnsigned { get; set; }

        public string CreatedBy { get; set; }
        public string? CreateByName { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }


    }
}
