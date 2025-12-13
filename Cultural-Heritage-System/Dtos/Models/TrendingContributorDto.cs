using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Models
{
    public class TrendingContributorDto
    {
        public int ContributorId { get; set; }
        public string ContributorName { get; set; }
        public string AvatarUrl { get; set; }
        public int TotalPosts { get; set; }
        public int TotalViews { get; set; }
        public int TotalComments { get; set; }
        public int TotalSaves { get; set; }
        public double Score { get; set; }

    }


}
