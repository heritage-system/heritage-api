using Cultural_Heritage_System.GameHubs;

namespace Cultural_Heritage_System.Dtos.Models
{
    public class WaitingPlayer
    {
        public string ConnectionId { get; set; } = "";
        public Player Player { get; set; } = new();
    }

}
