using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.User
{
    public class UserDetailResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? AvatarUrl { get; set; }
        public UserStatus UserStatus { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? CreatedBy { get; set; }   
        public string? UpdatedBy { get; set; }      
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }


        //static
        public int NumberOfFavorites { get; set; }
        public int NumberOfHeritageReviews { get; set; }
        public int NumberOfReports { get; set; }
        public int NumberOfSubscriptions { get; set; }
        public int NumberOfContributionSaves { get; set; }
        public int NumberOfContributionReviews { get; set; }
        public int NumberOfContributionReports { get; set; }

    }
}
