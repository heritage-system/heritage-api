using System.Text.Json;
using Cultural_Heritage_System.Dtos.AuthenticationProvider.Google;

namespace Cultural_Heritage_System.Repositories
{
    public class GoogleAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleAuthClient> _logger;

        public GoogleAuthClient(HttpClient httpClient, IConfiguration configuration, ILogger<GoogleAuthClient> _logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            this._logger = _logger;
        }

        public async Task<ExchangeTokenResponse> ExchangeTokenAsync(string code)
        {
            var requestBody = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _configuration["Authentication:Google:ClientId"]),
                new KeyValuePair<string, string>("client_secret", _configuration["Authentication:Google:ClientSecret"]),
                new KeyValuePair<string, string>("redirect_uri", _configuration["Authentication:Google:RedirectUri"]),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            ]);


            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", requestBody);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to exchange token: {error}", responseContent);
                throw new HttpRequestException("Error getting token");
            }

            var tokenResponse = JsonSerializer.Deserialize<ExchangeTokenResponse>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            _logger.LogInformation("Successfully obtained access token: {accessToken}", tokenResponse.AccessToken);
            return tokenResponse;
        }
    }
}