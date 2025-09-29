//using System.ComponentModel.DataAnnotations.Schema;
//using System.ComponentModel.DataAnnotations;
//using Cultural_Heritage_System.Common;

//namespace Cultural_Heritage_System.Models
//{
//    public class ContributionUnlock : BaseEntity<int>
//    {

//        [Required]
//        [Column("user_id")]
//        [ForeignKey("User")]
//        public int UserId { get; set; }
//        public User User { get; set; }

//        [Required]
//        [ForeignKey("Contribution")]
//        [Column("contribution_id")]
//        public int ContributionId { get; set; }
//        public Contribution Contribution { get; set; }


//    }

//}
