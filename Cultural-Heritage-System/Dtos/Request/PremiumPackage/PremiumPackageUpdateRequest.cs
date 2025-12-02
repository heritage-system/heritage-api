namespace Cultural_Heritage_System.Dtos.Request.PremiumPackage
{
    public class PremiumPackageUpdateRequest
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? DurationDays { get; set; }
        public string? MarketingMessage { get; set; }
        public bool? IsActive { get; set; }
        public List<int>? BenefitIds { get; set; }
    }

}
