using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Favorite: BaseEntity<int>
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

    }

}
