using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class ContributionHeritageTag : BaseEntity<long>
    {       

        [Required]
        [ForeignKey("Contribution")]
        [Column("contribution_id")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }

        [Required]
        [Column("heritage_id")]
        [ForeignKey("Heritage")]
        public long HeritageId { get; set; }
        public Heritage Heritage { get; set; }

    }

}
