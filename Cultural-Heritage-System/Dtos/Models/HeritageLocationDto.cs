using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;
using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Models
{
    public class HeritageLocationDto
    {
        public int Id { get; set; }     
        public string Name { get; set; }
        public string Code { get; set; }  
    }
}
