using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class MonthlySettlement: BaseEntity<int>
    {
        [Column("year")]
        public int Year { get; set; }

        [Column("month")]
        public int Month { get; set; }

        [Column("total_revenue")]
        public decimal TotalRevenue { get; set; }
        [Column("platform_fee")]
        public decimal PlatformFee { get; set; }          // phần platform giữ lại
        [Column("contributor_pool")]
        public decimal ContributorPool { get; set; }      // phần chia contributor
        [Column("finalized")]
        public bool Finalized { get; set; } = false;

    }

}
