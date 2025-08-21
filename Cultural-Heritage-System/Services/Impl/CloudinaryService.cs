using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Cultural_Heritage_System.Services.Impl
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.AbsoluteUri;
        }

        public async Task<string> UploadDocumentAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new RawUploadParams
            {
                File = new FileDescription(fileName, fileStream),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.AbsoluteUri ?? uploadResult.Url?.AbsoluteUri ?? string.Empty;
        }

        public async Task<string> UploadVideoAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(fileName, fileStream),
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.AbsoluteUri ?? uploadResult.Url?.AbsoluteUri ?? string.Empty;
        }
    }
}
