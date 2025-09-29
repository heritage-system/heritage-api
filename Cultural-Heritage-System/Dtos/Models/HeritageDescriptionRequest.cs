namespace Cultural_Heritage_System.Dtos.Models
{
    public class HeritageDescriptionRequest
    {
        public List<ContentBlock> History { get; set; } = new();
        public List<ContentBlock> Rituals { get; set; } = new();
        public List<ContentBlock> Values { get; set; } = new();
        public List<ContentBlock> Preservation { get; set; } = new();
    }

    public class ContentBlock
    {
        public string Type { get; set; }
        public string? Content { get; set; }
        public List<string>? Items { get; set; }
    }
}
