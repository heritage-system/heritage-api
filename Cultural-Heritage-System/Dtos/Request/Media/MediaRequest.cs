using Cultural_Heritage_System.Common;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Dtos.Request.Media
{

    public class MediaRequest
    {
        public string Url { get; set; }
        public MediaType MediaType { get; set; }           
        public string? Description { get; set; }
    }


}
