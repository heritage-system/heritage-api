using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [Route("api/v1/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<FileController> _logger;
        private readonly IConfiguration _configuration;

        public FileController(ICloudinaryService cloudinaryService, ILogger<FileController> logger, IConfiguration configuration)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("upload/image")]
        //[Authorize]
        public async Task<ApiResponse<string>> UploadImage(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var imageUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName);
                return new ApiResponse<string>
                {
                    code = 200,
                    message = "Image uploaded successfully.",
                    result = imageUrl
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                return new ApiResponse<string>
                {
                    code = 400,
                    message = "Image uploaded failed",
                };
            }
        }

        [HttpPost("upload/document")]
        //[Authorize]
        public async Task<ApiResponse<string>> UploadDocument(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var fileUrl = await _cloudinaryService.UploadDocumentAsync(stream, file.FileName);
                return new ApiResponse<string>
                {
                    code = 200,
                    message = "File uploaded successfully.",
                    result = fileUrl
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                return new ApiResponse<string>
                {
                    code = 400,
                    message = "File uploaded failed",
                };
            }
        }

        [HttpPost("upload/video")]
        //[Authorize]
        public async Task<ApiResponse<string>> UploadVideo(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ApiResponse<string>
                    {
                        code = 400,
                        message = "No file provided"
                    };
                }

                var allowedContentTypes = new[] { "video/mp4", "video/quicktime", "video/x-msvideo", "video/x-ms-wmv", "video/webm", "video/ogg" };
                if (!allowedContentTypes.Contains(file.ContentType))
                {
                    return new ApiResponse<string>
                    {
                        code = 400,
                        message = "Unsupported video type"
                    };
                }

                var maxSizeMb = _configuration.GetValue<int>("Cloudinary:MaxVideoSizeMB", 50);
                var maxBytes = maxSizeMb * 1024L * 1024L;
                if (file.Length > maxBytes)
                {
                    return new ApiResponse<string>
                    {
                        code = 400,
                        message = $"File too large. Max {maxSizeMb}MB"
                    };
                }

                using var stream = file.OpenReadStream();
                var videoUrl = await _cloudinaryService.UploadVideoAsync(stream, file.FileName);
                return new ApiResponse<string>
                {
                    code = 200,
                    message = "Video uploaded successfully.",
                    result = videoUrl
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video");
                return new ApiResponse<string>
                {
                    code = 400,
                    message = "Video uploaded failed",
                };
            }
        }

    }
}
