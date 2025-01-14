using System.Security.Claims;
using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Identity.Core.Interfaces;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokens(string  userId, IEnumerable<string> roles);
    Task<ClaimsPrincipal> ValidateToken(string token);
    Task RevokeToken(string                    token);
}