using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.User
{
    public class UserSearchRequest
    {
        public string? Keyword { get; set; }
        public UserStatus? Status { get; set; }
        public string Role { get; set; } = DefinitionRole.MEMBER;
        public SortBy SortBy { get; set; } = SortBy.IDASC;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
