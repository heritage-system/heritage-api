using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class SystemLog: BaseEntity<long>
    {
      
        [Column("user_id")]
        [ForeignKey("User")]       
        public int? UserId { get; set; }
        public User? User { get; set; }

        [Column("action")]
        public SystemLogAction Action { get; set; }

        [Column("details")]
        public string? Details { get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

    }

}
