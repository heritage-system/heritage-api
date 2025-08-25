using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeritageController : ControllerBase
    {
        private readonly IHeritageService _heritageService;

        public HeritageController(IHeritageService heritageService)
        {
            _heritageService = heritageService;
        }

        [HttpGet("heritage/all")]
        public async Task<IActionResult> GetAll()
        {
            var heritages = await _heritageService.GetAllAsync();
            return Ok(heritages);
        }

        [HttpGet("heritage/id")]
        public async Task<IActionResult> GetById([FromQuery] long id)
        {
            var heritage = await _heritageService.GetByIdAsync(id);
            if (heritage == null)
                return NotFound(new { message = "Heritage not found" });

            return Ok(heritage);
        }

        [HttpPost("heritage/create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] HeritageCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newHeritage = await _heritageService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = newHeritage.Id }, newHeritage);
        }

        [HttpPut("heritage/Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromQuery]long id, [FromBody] HeritageUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedHeritage = await _heritageService.UpdateAsync(id, request);
            if (updatedHeritage == null)
                return NotFound(new { message = "Heritage not found" });

            return Ok(updatedHeritage);
        }

        [HttpDelete("heritage/delete")]
        [Authorize]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var result = await _heritageService.DeleteAsync(id);
            if (!result)
                return NotFound(new { message = "Heritage not found" });

            return NoContent();
        }
    }
}
