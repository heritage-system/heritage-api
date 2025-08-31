using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Category : BaseEntity<int>
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }
        [Column("name_unsigned")]
        public string NameUnsigned { get; set; }

        [Column("description_unsigned")]
        public string DescriptionUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            NameUnsigned = StringHelper.RemoveDiacritics(Name).ToLower();
            DescriptionUnsigned = StringHelper.RemoveDiacritics(Description).ToLower();
        }

        public ICollection<Heritage> Heritages { get; set; } = new List<Heritage>();
    }

}
