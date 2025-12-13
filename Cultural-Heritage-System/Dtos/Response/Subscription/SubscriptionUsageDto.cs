namespace Cultural_Heritage_System.Dtos.Response.Subscription
{
    public class SubscriptionUsageDto
    {
        public long Id { get; set; }

        public string? BenefitName { get; set; }

        public int? Total { get; set; }
        public int Used { get; set; }

        public string Status { get; set; }
    }
}
