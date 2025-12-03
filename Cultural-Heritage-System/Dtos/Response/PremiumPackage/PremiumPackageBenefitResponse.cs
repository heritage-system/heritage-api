using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.PremiumPackage
{
    public class PremiumPackageBenefitResponse
    {
        public int BenefitId { get; set; }
        public BenefitName BenefitName { get; set; }
        public BenefitType BenefitType { get; set; }
        public int? Value { get; set; }
    }

}
