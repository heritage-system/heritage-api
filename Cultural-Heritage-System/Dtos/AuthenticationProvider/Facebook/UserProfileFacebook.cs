using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Dtos.AuthenticationProvider.Facebook
{
    public class UserProfileFacebook
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("picture")]
        public PictureData Picture { get; set; }
    }

    public class PictureData
    {
        [JsonPropertyName("data")]
        public PictureInfo Data { get; set; }
    }

    public class PictureInfo
    {
        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("is_silhouette")]
        public bool IsSilhouette { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

}