using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Tinker.Infrastructure.Identity.Core.Interfaces;
using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Identity.Core.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly IDistributedCache _cache;

    public TokenService(IConfiguration configuration, IDistributedCache cache)
    {
        _configuration = configuration;
        _cache = cache;
    }

    public async Task<TokenResponse> GenerateTokens(string userId, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var jwtKey = _configuration.GetValue<string>("Jwt:Key") ?? throw new InvalidOperationException("JWT key not configured");
        // Use the key from a secure configuration source
        var key = new SymmetricSecurityKey(Convert.FromBase64String(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var refreshToken = GenerateRefreshToken();
        return new TokenResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken
        );
    }

    public async Task<ClaimsPrincipal> ValidateToken(string token)
    {
        var isRevoked = await _cache.GetAsync($"revoked_{token}") != null;
        if (isRevoked)
            throw new SecurityTokenException("Token has been revoked");

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtKey = _configuration.GetValue<string>("Jwt:Key") ?? throw new InvalidOperationException("JWT key not configured");
        var key = new SymmetricSecurityKey(Convert.FromBase64String(jwtKey));

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = key
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch (Exception)
        {
            throw new SecurityTokenException("Invalid token");
        }
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task RevokeToken(string token)
    {
        await _cache.SetStringAsync($"revoked_{token}", "true", new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        });
    }
}
