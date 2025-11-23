using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PremiumPackageBenefit : BaseEntity<int>
    {
        [Column("premium_package_id")]
        [ForeignKey("PremiumPackage")]
        public int PackageId { get; set; }
        public PremiumPackage Package { get; set; }

        [Column("premium_benefit_id")]
        [ForeignKey("PremiumBenefit")]
        public int BenefitId { get; set; }
        public PremiumBenefit Benefit { get; set; }
    }
}
