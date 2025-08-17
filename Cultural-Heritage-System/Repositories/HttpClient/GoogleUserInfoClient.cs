using System.Text.Json;
using Cultural_Heritage_System.Dtos.AuthenticationProvider.Google;

namespace Cultural_Heritage_System.Repositories
{
    public class GoogleUserInfoClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleUserInfoClient> logger;

        public GoogleUserInfoClient(HttpClient httpClient, ILogger<GoogleUserInfoClient> logger)
        {
            _httpClient = httpClient;
            this.logger = logger;
        }

        public async Task<OutboundUserResponse> GetUserInfoAsync(string accessToken)
        {
            logger.LogInformation("Access token {}", accessToken);
            var response = await _httpClient.GetAsync($"oauth2/v1/userinfo?alt=json&access_token={accessToken}");
            var responseContent = await response.Content.ReadAsStringAsync();


            return JsonSerializer.Deserialize<OutboundUserResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new Exception("Failed to deserialize user info");
        }
    }
}