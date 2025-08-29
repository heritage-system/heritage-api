namespace Cultural_Heritage_System.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName);
        Task<string> UploadDocumentAsync(Stream fileStream, string fileName);
        Task<string> UploadVideoAsync(Stream fileStream, string fileName);
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string mediaType, string folder);
    }
}
