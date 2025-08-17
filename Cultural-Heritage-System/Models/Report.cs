using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Report: BaseEntity<long>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [Column("heritage_id")]
        [ForeignKey("Heritage")]      
        public long HeritageId { get; set; }
        public Heritage Heritage { get; set; }

        [Required]
        [Column("reason")]
        public string Reason { get; set; }

    }

}
