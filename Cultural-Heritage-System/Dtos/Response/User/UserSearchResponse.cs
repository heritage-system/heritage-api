using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.User
{
    public class UserSearchResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }           
        public UserStatus UserStatus { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; }


    }
}
