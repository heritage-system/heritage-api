using System.ComponentModel.DataAnnotations;

namespace Cultural_Heritage_System.Dtos.Request
{
    public class LocationCreateRequest
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

}
