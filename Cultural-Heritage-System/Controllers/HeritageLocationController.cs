using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/heritageLocation")]
    [ApiController]
    public class HeritageLocationController : ControllerBase
    {
        private readonly IHeritageLocationService heritageService;

        public HeritageLocationController(IHeritageLocationService heritageService)
        {
            this.heritageService = heritageService;
        }

        // GET api/v1/heritages/map
        [HttpGet("map")]
        public async Task<ApiResponse<List<HeritageLocationResponse>>> GetHeritagesWithMap()
        {
            var result = await heritageService.GetHeritagesWithCoordinates();
            return new ApiResponse<List<HeritageLocationResponse>>(200, "Success", result);
        }
    }
}
