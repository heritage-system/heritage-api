namespace Cultural_Heritage_System.Dtos.Response
{
    public class PageResponse<T>
    {
        public int CurrentPages { get; set; }
        public int PageSizes { get; set; }
        public int TotalPages { get; set; }
        public long TotalElements { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }
}
