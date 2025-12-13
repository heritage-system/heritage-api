using Cultural_Heritage_System.GameHubs;

namespace Cultural_Heritage_System.Dtos.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public string? IpAddress { get; set; }
        public string? ConnectionId { get; set; }
        public int Score { get; set; } = 0;
        public double LastAnswerTime { get; set; } = 0;
        public int? BonusPoint { get; set; } = 0;
    }

    

}
