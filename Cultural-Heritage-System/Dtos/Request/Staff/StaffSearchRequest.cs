using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.Staff
{
    public class StaffSearchRequest
    {
        public string? Keyword { get; set; }
        public StaffStatus? Status { get; set; }
        public StaffRole? StaffRole { get; set; }
        public SortBy SortBy { get; set; } = SortBy.IDASC;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
