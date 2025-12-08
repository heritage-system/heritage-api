using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class GameMatchHistory : BaseEntity<long>
    {
        // ----------- Match Info -----------
        [Required]
        [Column("match_id", TypeName = "uniqueidentifier")]
        public Guid MatchId { get; set; }

        [Required]
        [Column("match_type", TypeName = "nvarchar(20)")]
        public RoomType MatchType { get; set; } = RoomType.RANDOM;

        [Required]
        [Column("question_count")]
        public int QuestionCount { get; set; }      


        // ----------- Player 1 -----------
        [Required]
        [ForeignKey("Player1")]
        [Column("player_1_id")]
        public int Player1Id { get; set; }
        public User? Player1 { get; set; }

        [Column("player_1_name")]
        public string Player1Name { get; set; }

        [Column("player_1_avatar")]
        public string? Player1Avatar { get; set; }

        [Column("player_1_score")]
        public int Player1Score { get; set; }

        [Column("player_1_ip", TypeName = "nvarchar(64)")]
        public string? Player1IP { get; set; }


        // ----------- Player 2 -----------
        [ForeignKey("Player2")]
        [Column("player_2_id")]
        public int? Player2Id { get; set; } // null nếu bot
        public User? Player2 { get; set; }

        [Column("player_2_name")]
        public string Player2Name { get; set; }
        [Column("player_2_avatar")]
        public string? Player2Avatar { get; set; }

        [Column("player_2_score")]
        public int Player2Score { get; set; }      

        [Column("player_2_ip", TypeName = "nvarchar(64)")]
        public string? Player2IP { get; set; }


        // ----------- Result -----------
        [Column("winner", TypeName = "nvarchar(20)")]
        public Winner? WinnerPlayer { get; set; }

        [Column("plus_point")]
        public int? PlusPoint { get; set; }

        [Column("cheating_risk_score")]
        public int CheatingRiskScore { get; set; } = 0;
    }



}
