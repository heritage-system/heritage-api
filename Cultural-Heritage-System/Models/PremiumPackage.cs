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
        [Required]
        [Column("max_opens_per_month")]
        public int MaxOpensPerMonth { get; set; }
        [Column("currency", TypeName = "char(3)")]
        public string Currency { get; set; } = "VND";
        public bool IsActive { get; set; } = true;
              
    }


}
