using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class Contribution: BaseEntity<int>
    {
        [Required]
        [Column("user_id")]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("media_url")]
        public string MediaUrl { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("reviewed_by")]
        [ForeignKey(nameof(Reviewer))]
        public int ReviewedBy { get; set; }
        public User Reviewer { get; set; }
    }

}
