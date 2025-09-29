using Cultural_Heritage_System.Dtos.Response.Location;
using Cultural_Heritage_System.Dtos.Response.Media;
using Cultural_Heritage_System.Dtos.Response.Occurence;
using Cultural_Heritage_System.Dtos.Response.Tag;

namespace Cultural_Heritage_System.Dtos.Response.Contribution
{
    public class TopContributionHeritageTagResponse
    {
        public long HeritageId { get; set; }
        public string HeritageName { get; set; }
        public int Count { get; set; }
    }

}
