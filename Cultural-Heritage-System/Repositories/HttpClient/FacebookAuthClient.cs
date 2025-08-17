using Cultural_Heritage_System.Dtos.AuthenticationProvider.Facebook;
using System.Text.Json;

namespace Cultural_Heritage_System.Repositories
{
    public class FacebookAuthClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FacebookAuthClient> _logger;

        public FacebookAuthClient(HttpClient httpClient, IConfiguration configuration, ILogger<FacebookAuthClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ExchangeTokenFacebook?> ExchangeTokenAsync(string code)
        {
            try
            {
                var clientId = _configuration["Authentication:Facebook:ClientId"];
                var clientSecret = _configuration["Authentication:Facebook:ClientSecret"];
                var redirectUri = _configuration["Authentication:Facebook:RedirectUri"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
                {
                    _logger.LogError("Facebook OAuth configuration is missing");
                    return null;
                }

                var parameters = new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["redirect_uri"] = redirectUri,
                    ["code"] = code
                };

                var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
                var requestUrl = $"v23.0/oauth/access_token?{queryString}";

                _logger.LogInformation("Exchanging Facebook authorization code for access token");

                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to exchange Facebook token. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<ExchangeTokenFacebook>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                _logger.LogInformation("Successfully exchanged Facebook authorization code for access token");
                return tokenResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while exchanging Facebook token");
                return null;
            }
        }
    }

}