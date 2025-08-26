using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class HeritageMediaDto 
    {
        public long Id { get; set; }                
        public string MediaTypeName { get; set; }
        public string Url { get; set; }
        public string? Description { get; set; }

       
    }

}
