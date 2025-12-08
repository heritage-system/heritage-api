
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;


namespace Cultural_Heritage_System.Services
{
    public interface IGameMatchHistoryService
    {
        Task<bool> CreateGameMatchHistory(GameMatchHistory request);
        Task<bool> IsSpamMatch(int userId, string userIP, int opponentId, string opponentIP);
    }
}
