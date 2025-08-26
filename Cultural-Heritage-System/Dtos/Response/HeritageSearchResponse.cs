using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class HeritageSearchResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
      
        public string Description { get; set; }

        public string CategoryName { get; set; }
            
        public string MapUrl { get; set; }
       
        public bool IsFeatured { get; set; }

        public List<HeritageOccurrenceDto> HeritageOccurrences { get; set; }
        public List<HeritageMediaDto> Media { get; set; }
        public List<string> HeritageTags { get; set; }
        public List<HeritageLocationDto> HeritageLocations { get; set; }
        public string NameUnsigned { get; set; }

        public string DescriptionUnsigned { get; set; }      

    }
}
