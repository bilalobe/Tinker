using Microsoft.AspNetCore.Identity.Data;
using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Identity.Core.Interfaces;

public interface IAuthService
{
    Task<TokenResponse> Login(LoginRequest               request);
    Task<TokenResponse> RefreshToken(string              refreshToken);
    Task<bool> ValidateTokenAsync(TokenValidationContext context);
    Task RevokeToken(string                              token);
}