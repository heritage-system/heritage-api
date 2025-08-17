using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Models
{
    public abstract class BaseEntity<T>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public T Id { get; set; }

        [Column("created_by")]
        public string? CreatedBy { get; set; }

        [Column("updated_by")]
        public string? UpdatedBy { get; set; }

        [Column("create_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("update_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
