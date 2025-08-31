using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Tag : BaseEntity<int>
    {
        [Column("name")]
        public string Name { get; set; }
        [Column("name_unsigned")]
        public string NameUnsigned { get; set; }


        public void GenerateUnsignedFields()
        {
            NameUnsigned = StringHelper.RemoveDiacritics(Name).ToLower();

        }

        public ICollection<HeritageTag> HeritageTags { get; set; } = new List<HeritageTag>();
    }

}
