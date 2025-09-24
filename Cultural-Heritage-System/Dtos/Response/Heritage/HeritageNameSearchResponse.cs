using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Response.Heritage
{
    public class HeritageNameSearchResponse
    {
        public long Id { get; set; }      
        public string Name { get; set; }            
    }
}
