namespace Cultural_Heritage_System.Dtos.Response
{
    public class FavoriteHeritageResponse
    {
        public long HeritageId { get; set; }
        public string HeritageName { get; set; }
        public string HeritageDescription { get; set; }
        public string CategoryName {  get; set; }
        public bool IsFeatured {  get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
