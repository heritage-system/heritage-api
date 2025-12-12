using Cultural_Heritage_System.Dtos.Request.PremiumBenefit;
using Cultural_Heritage_System.Dtos.Request.PremiumPackage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.PremiumBenefit;
using Cultural_Heritage_System.Dtos.Response.PremiumPackage;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("/api/v1/PremiumBenefit")]
    [ApiController]
    public class PremiumBenefitController : ControllerBase
    {
        private readonly IPremiumBenefitService _service;

        public PremiumBenefitController(IPremiumBenefitService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<PremiumBenefitResponse>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return new ApiResponse<IEnumerable<PremiumBenefitResponse>>(200, "Fetched all benefits successfully", result);
        }

        [HttpGet("byId")]
        public async Task<ApiResponse<PremiumBenefitResponse>> GetById([FromQuery]int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return new ApiResponse<PremiumBenefitResponse>(404, "Benefit not found", null);
            return new ApiResponse<PremiumBenefitResponse>(200, "Fetched benefit successfully", result);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<ApiResponse<PremiumBenefitResponse>>> Create([FromForm] PremiumBenefitCreateRequest req)
        {
            var result = await _service.CreateAsync(req);
            return StatusCode(200, new ApiResponse<PremiumBenefitResponse>(
                code: 200,
                message: "Created benefit successfully",
                result: result
            ));
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<PremiumBenefitResponse>> Update([FromQuery] int id, [FromBody] PremiumBenefitUpdateRequest req)
        {
            var result = await _service.UpdateAsync(id, req);
            return new ApiResponse<PremiumBenefitResponse>(200, "Updated benefit successfully", result);
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ApiResponse<object>> Delete([FromQuery] int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return new ApiResponse<object>(404, "Benefit not found", null);
            return new ApiResponse<object>(200, "Deleted benefit successfully", null);
        }

    }

}
