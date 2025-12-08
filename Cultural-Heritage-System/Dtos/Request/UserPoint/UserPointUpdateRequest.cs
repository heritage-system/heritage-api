using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.UserPoint
{
    public class UserPointUpdateRequest
    {
        public int UserId { get; set; }
        public int ChangeAmount { get; set; }      
        public PointHistoriesReason Reason { get; set; }      
        public long ReferenceId { get; set; }
    }
}
