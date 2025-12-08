using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.UserPoint
{
    public class UserPointResponse
    {
        public int UserId { get; set; }     
        public int TotalPoints { get; set; }
        public int UnlockTokens { get; set; }
    }
}
