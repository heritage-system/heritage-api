using Cultural_Heritage_System.Dtos.AuthenticationProvider.Facebook;
using System.Text.Json;

namespace Cultural_Heritage_System.Repositories
{
    public class FacebookUserInfoClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FacebookUserInfoClient> _logger;

        public FacebookUserInfoClient(HttpClient httpClient, ILogger<FacebookUserInfoClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserProfileFacebook?> GetUserProfileAsync(string accessToken)
        {
            try
            {
                var profileUrl = "me?fields=id,name,picture";

                _logger.LogInformation("Fetching Facebook user profile");

                using var request = new HttpRequestMessage(HttpMethod.Get, profileUrl);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get Facebook user profile. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var userProfile = JsonSerializer.Deserialize<UserProfileFacebook>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Successfully retrieved Facebook user profile for user ID: {UserId}", userProfile?.Id);
                return userProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting Facebook user profile");
                return null;
            }
        }

        public async Task<UserProfileFacebook?> GetUserProfileWithEmailAsync(string accessToken)
        {
            try
            {
                var profileUrl = "me?fields=id,name,email,picture";

                _logger.LogInformation("Fetching Facebook user profile with email");

                using var request = new HttpRequestMessage(HttpMethod.Get, profileUrl);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to get Facebook user profile with email. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var userProfile = JsonSerializer.Deserialize<UserProfileFacebook>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogInformation("Successfully retrieved Facebook user profile with email for user ID: {UserId}", userProfile?.Id);
                return userProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting Facebook user profile with email");
                return null;
            }
        }
    }
}