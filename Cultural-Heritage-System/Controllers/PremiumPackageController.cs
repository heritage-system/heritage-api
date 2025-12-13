using Cultural_Heritage_System.Dtos.Request.PremiumPackage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.PremiumPackage;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/PremiumPackage")]
    [ApiController]
    public class PremiumPackageController : ControllerBase
    {
        private readonly IPremiumPackageService _packageService;

        public PremiumPackageController(IPremiumPackageService service)
        {
            _packageService = service;
        }

        [HttpGet]
        public async Task<ApiResponse<IEnumerable<PremiumPackageResponse>>> GetAll()
        {
            var result = await _packageService.GetAllAsync();
            return new ApiResponse<IEnumerable<PremiumPackageResponse>>(
                code: 200,
                message: "Fetched all premium packages successfully",
                result: result
            );
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<PageResponse<PremiumPackageResponse>>>> Search(
            [FromQuery] PremiumPackageSearchRequest request)
        {
            try
            {
                var result = await _packageService.SearchPremiumPackagesAsync(request);

                return Ok(new ApiResponse<PageResponse<PremiumPackageResponse>>(
                    code: 200,
                    message: "Search premium packages successfully",
                    result: result
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(
                    code: 500,
                    message: "An error occurred while searching premium packages",
                    result: ex.Message
                ));
            }
        }

        [HttpGet("byId")]
        public async Task<ActionResult<ApiResponse<PremiumPackageResponse>>> GetById([FromQuery] int id)
        {
            var result = await _packageService.GetByIdAsync(id);

            if (result == null)
                return NotFound(new ApiResponse<PremiumPackageResponse>(
                    code: 404,
                    message: "Premium package not found",
                    result: null
                ));

            return Ok(new ApiResponse<PremiumPackageResponse>(
                code: 200,
                message: "Fetched premium package successfully",
                result: result
            ));
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<ApiResponse<PremiumPackageResponse>>> Create(
            [FromForm] PremiumPackageCreateRequest request)
        {
            var result = await _packageService.CreateAsync(request);

            return StatusCode(201, new ApiResponse<PremiumPackageResponse>(
                code: 201,
                message: "Created premium package successfully",
                result: result
            ));
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<ApiResponse<PremiumPackageResponse>>> Update(
            [FromQuery] int id,
            [FromForm] PremiumPackageUpdateRequest request)
        {
            var result = await _packageService.UpdateAsync(id, request);

            if (result == null)
                return NotFound(new ApiResponse<PremiumPackageResponse>(
                    code: 404,
                    message: "Premium package not found",
                    result: null
                ));

            return Ok(new ApiResponse<PremiumPackageResponse>(
                code: 200,
                message: "Updated premium package successfully",
                result: result
            ));
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<ActionResult<ApiResponse<object>>> Delete([FromQuery] int id)
        {
            var success = await _packageService.DeleteAsync(id);

            if (!success)
                return NotFound(new ApiResponse<object>(
                    code: 404,
                    message: "Premium package not found",
                    result: null
                ));

            return Ok(new ApiResponse<object>(
                code: 200,
                message: "Deleted premium package successfully",
                result: null
            ));
        }

        [HttpGet("byActive")]
        public async Task<ApiResponse<PremiumPackageListResponse>> GetAllActivePackage()
        {
            var result = await _packageService.GetActivePackagesAsync();

            return new ApiResponse<PremiumPackageListResponse>(
                code: 200,
                message: "Fetched all premium packages successfully",
                result: result
            );
        }

    }
}
