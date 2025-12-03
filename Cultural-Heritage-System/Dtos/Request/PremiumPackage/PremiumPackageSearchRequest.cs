using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.PremiumPackage
{
    public class PremiumPackageSearchRequest
    {
        public string? Name { get; set; }
        public SortBy? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
