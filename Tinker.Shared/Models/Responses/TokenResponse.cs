namespace Tinker.Shared.Models.Responses;
public class TokenResponse
{
    public TokenResponse(string v, string refreshToken)
    {
        V = v;
        RefreshToken = refreshToken;
    }

    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string V { get; }
}