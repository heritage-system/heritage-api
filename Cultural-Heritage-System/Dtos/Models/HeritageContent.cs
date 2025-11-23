namespace Cultural_Heritage_System.Dtos.Models
{
    public class HeritageContent
    {
        public List<HeritageDescriptionBlock> History { get; set; } = new();
        public List<HeritageDescriptionBlock> Rituals { get; set; } = new();
        public List<HeritageDescriptionBlock> Values { get; set; } = new();
        public List<HeritageDescriptionBlock> Preservation { get; set; } = new();
    }

    public class HeritageDescriptionBlock
    {
        public string Type { get; set; }           // "paragraph" | "list"
        public string? Content { get; set; }       // nội dung đoạn văn
        public List<string>? Items { get; set; }   // danh sách bullet
    }

}
