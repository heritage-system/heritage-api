using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class HeritageSearchRequest
    {
        public string? Keyword { get; set; }

        public List<int>? CategoryIds { get; set; }
        public List<int>? TagIds { get; set; }
        public List<int>? LocationIds { get; set; }

        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public double? Radius { get; set; }     
        public int? StartDay { get; set; }
     
        public int? StartMonth { get; set; }
     
        public int? EndDay { get; set; }
      
        public int? EndMonth { get; set; }

        public CalendarType CalendarType { get; set; } = CalendarType.SOLAR;

        public SortBy? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

    }
}
