using System.Security.Claims;

namespace Cultural_Heritage_System.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(Claim[] claims);
        string GenerateRefreshToken(Claim[] claims);
    }
}
