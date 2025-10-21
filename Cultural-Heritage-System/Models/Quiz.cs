using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class Quiz : BaseEntity<long>, IUnsignedEntity
    {

        [Column("title")]
        public string Title { get; set; }

        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public ICollection<QuizResult> Results { get; set; } = new List<QuizResult>();

        [Column("title_unsigned")]
        public string TitleUnsigned { get; set; }
        public void GenerateUnsignedFields()
        {
            TitleUnsigned = StringHelper.RemoveDiacritics(Title).ToLower();         
        }
    }

}
