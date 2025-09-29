using Cultural_Heritage_System.Common;

public class SignInResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? UserType { get; set; }
    public string? TokenType { get; set; }
    public string? UserName { get; set; }
    public string? AvatarUrl { get; set; }
    public TwoFaStep TwoFaStep { get; set; } = TwoFaStep.NONE;

    public SignInResponse(string accessToken, string refreshToken, string userType, string tokenType, TwoFaStep twoFaStep,string username, string avatarUrl)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        UserType = userType;
        TokenType = tokenType;
        TwoFaStep = twoFaStep;
        UserName = username;
        AvatarUrl = avatarUrl;
    }

    public SignInResponse(TwoFaStep step)
    {
        TwoFaStep = step;
    }
}
