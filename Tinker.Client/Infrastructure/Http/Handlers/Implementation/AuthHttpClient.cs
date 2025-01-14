using System.Net.Http.Json;

namespace Tinker.Client.Infrastructure.Http.Handlers.Implementation;

public class AuthHttpClient(HttpClient httpClient) : IAuthHttpClient
{
    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", request);
        return await response.Content.ReadFromJsonAsync<AuthResult>()
               ?? new AuthResult { Succeeded = false };
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/refresh", new { token = refreshToken });
        return await response.Content.ReadFromJsonAsync<AuthResult>()
               ?? new AuthResult { Succeeded = false };
    }

    public async Task LogoutAsync()
    {
        await httpClient.PostAsync("api/auth/logout", null);
    }

    public async Task<UserInfo> GetUserInfoAsync()
    {
        return await httpClient.GetFromJsonAsync<UserInfo>("api/auth/user")
               ?? new UserInfo { IsAuthenticated = false };
    }
}