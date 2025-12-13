using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class PremiumPackage : BaseEntity<int>
    {
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Required]
        [Column("price", TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Column("currency", TypeName = "char(3)")]
        public string Currency { get; set; } = "VND";

        [Column("duration_days")]
        public int? DurationDays { get; set; }

        [Column("marketing_message")]
        public string? MarketingMessage { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public ICollection<PremiumPackageBenefit> PackageBenefits { get; set; } = new List<PremiumPackageBenefit>();

    }


}
