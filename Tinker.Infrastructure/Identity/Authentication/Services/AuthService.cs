using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tinker.Infrastructure.Identity.Core.Interfaces;
using Tinker.Infrastructure.Identity.Core.Models;
using Tinker.Shared.Models.Responses;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<TokenResponse> Login(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username)
            ?? throw new AuthenticationException("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            throw new AuthenticationException("Invalid credentials");

        var roles = await _userManager.GetRolesAsync(user);
        return await _tokenService.GenerateTokens(user.Id, roles);
    }

    public async Task<TokenResponse> RefreshToken(string refreshToken)
    {
        var principal = await _tokenService.ValidateToken(refreshToken);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new SecurityTokenException("Invalid token");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new SecurityTokenException("User not found");

        var roles = await _userManager.GetRolesAsync(user);
        return await _tokenService.GenerateTokens(user.Id, roles);
    }

    public async Task<bool> ValidateTokenAsync(TokenValidationContext context)
    {
        var principal = context.Principal;
        if (principal?.Identity?.IsAuthenticated != true)
            return false;

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return false;

        var user = await _userManager.FindByIdAsync(userId);
        return user != null && user.IsActive;
    }

    public async Task RevokeToken(string token)
    {
        await _tokenService.RevokeToken(token);
    }
}