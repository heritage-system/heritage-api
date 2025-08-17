using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ExportLog: BaseEntity<int>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]       
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("format")]
        public string Format { get; set; }

        [Column("filter_criteria")]
        public string FilterCriteria { get; set; }

    }

}
