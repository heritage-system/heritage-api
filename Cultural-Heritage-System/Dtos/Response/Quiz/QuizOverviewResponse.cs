using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Models;

namespace Cultural_Heritage_System.Models
{
    public class QuizOverviewResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int NumberOfClear { get; set; }
        public int NumberOfQuestion { get; set; }
        public SubscriptionDto? Subscription { get; set; }
        public int? UserPoint { get; set; }
        public bool UnSubscriptionLock { get; set; } = false;     
        public bool IsUnlock = false;
    }

}
