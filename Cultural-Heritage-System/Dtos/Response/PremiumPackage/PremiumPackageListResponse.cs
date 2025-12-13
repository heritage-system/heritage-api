namespace Cultural_Heritage_System.Dtos.Response.PremiumPackage
{
    public class PremiumPackageListResponse
    {
        public int? CurrentPackageId { get; set; }

        public List<PremiumPackageResponse> PremiumPackages { get; set; } = new List<PremiumPackageResponse>();
    }

}
