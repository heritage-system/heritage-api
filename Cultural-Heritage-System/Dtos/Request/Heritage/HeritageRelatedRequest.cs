using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Heritage
{
    public class HeritageRelatedRequest
    {
        public string? Keyword { get; set; }
        public int CategoryIds { get; set; }
        public List<int>? TagIds { get; set; }
        public List<string>? Locations { get; set; }    
        public double? Radius { get; set; }     
        public long HeritageId { get; set; }

        public int Quantity { get; set; } = 6;

    }
}
