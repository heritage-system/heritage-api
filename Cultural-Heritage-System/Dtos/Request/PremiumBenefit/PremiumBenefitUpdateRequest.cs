using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Request.PremiumBenefit
{
    public class PremiumBenefitUpdateRequest
    {
        public BenefitName? BenefitName { get; set; }
        public BenefitType? BenefitType { get; set; }
        public int? Value { get; set; }
    }
}
