namespace Cultural_Heritage_System.Dtos.Response.PremiumPackage
{
    public class PremiumPackageResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int? DurationDays { get; set; }
        public string? MarketingMessage { get; set; }
        public bool IsActive { get; set; }

        public List<PremiumPackageBenefitResponse> Benefits { get; set; } = new List<PremiumPackageBenefitResponse>();
    }

}
