using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Models
{
    [Table("roles")]
    public class Role : BaseEntity<int>
    {
        [Required]
        [StringLength(50)]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
