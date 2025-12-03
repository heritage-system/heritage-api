using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.PremiumBenefit
{
    public class PremiumBenefitResponse
    {
        public int Id { get; set; }
        public BenefitName BenefitName { get; set; }
        public BenefitType BenefitType { get; set; }
        public int? Value { get; set; }
    }

}
