using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Dtos.Response.PremiumPackage;

namespace Cultural_Heritage_System.Dtos.Response.Subscription
{
    public class SubscriptionResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int PackageId { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public string Status { get; set; }

        public PremiumPackageResponse Package { get; set; }

        public List<SubscriptionPaymentDto> Payments { get; set; }
        public List<SubscriptionUsageDto> UsageRecords { get; set; }
    }
}
