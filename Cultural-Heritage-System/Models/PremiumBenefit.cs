using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PremiumBenefit : BaseEntity<int>
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("benefit_type", TypeName = "nvarchar(50)")]
        public BenefitType BenefitType { get; set; }
        public int? Value { get; set; } // số lượt nếu là LimitIncrease
    }

    

}
