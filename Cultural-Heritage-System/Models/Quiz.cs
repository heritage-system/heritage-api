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
        [Column("banner_url")]
        public string BannerUrl { get; set; }
       
        [Column("premium_type", TypeName = "nvarchar(30)")]
        public PremiumType PremiumType { get; set; } = PremiumType.FREE;

        [Column("title_unsigned")]
        public string TitleUnsigned { get; set; }
        public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public ICollection<QuizResult> Results { get; set; } = new List<QuizResult>();

        public void GenerateUnsignedFields()
        {
            TitleUnsigned = StringHelper.RemoveDiacritics(Title).ToLower();         
        }
    }

}
