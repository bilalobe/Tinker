using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using Tinker.Core.Security.Interfaces;

namespace Tinker.Client.Infrastructure.Auth;

public class AuthenticationService(
    IAuthHttpClient             authClient,
    AuthenticationStateProvider authStateProvider,
    ILocalStorageService        localStorage,
    HttpClient                  httpClient)
    : IAuthenticationService
{
    private readonly IAuthHttpClient _authClient = authClient;
    private readonly ILocalStorageService _localStorage = localStorage;

    public async Task<bool> LoginAsync(LoginRequest request)
    {
        var result = await _authClient.LoginAsync(request);
        if (result.Succeeded)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Token);

            ((CustomAuthStateProvider)authStateProvider).MarkUserAsAuthenticated(request.Username);
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");
        httpClient.DefaultRequestHeaders.Authorization = null;
        ((CustomAuthStateProvider)authStateProvider).MarkUserAsLoggedOut();
        await _authClient.LogoutAsync();
    }

    public async Task<bool> RefreshTokenAsync()
    {
        var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");
        if (string.IsNullOrEmpty(refreshToken))
            return false;

        var result = await _authClient.RefreshTokenAsync(refreshToken);
        if (result.Succeeded)
        {
            await _localStorage.SetItemAsync("authToken", result.Token);
            await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Token);
            return true;
        }

        await LogoutAsync();
        return false;
    }
}