using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.GameMatchHistory
{
    public class UserMatchHistoryResponse
    {       
        public Guid MatchId { get; set; }      
        public RoomType MatchType { get; set; }
        public int Player1Id { get; set; }
        public string Player1Name { get; set; }     
        public string? Player1Avatar { get; set; }     
        public int Player1Score { get; set; }
        public int? Player2Id { get; set; }
        public string Player2Name { get; set; }     
        public string? Player2Avatar { get; set; }
        public int Player2Score { get; set; }
        public Winner? WinnerPlayer { get; set; }
        public int UserNumber { get; set; }
        public int? PlusPoint { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
