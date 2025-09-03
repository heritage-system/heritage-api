using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Profile: BaseEntity<int>, IUnsignedEntity
    {
        [Required]
        [Column("user_id")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }


        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("avatar_url")]
        public string? AvatarUrl { get; set; }

        [Column("full_name_unsigned")]
        public string FullNameUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            FullNameUnsigned = StringHelper.RemoveDiacritics(FullName).ToLower();
        }
    }

}
