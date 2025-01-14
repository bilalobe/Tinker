using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tinker.Infrastructure.Identity.Core.Interfaces;
using Tinker.Infrastructure.Identity.Core.Models;
using Tinker.Shared.Exceptions;
using Tinker.Shared.Models.Auth;
using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Identity.Core.Services;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityService> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IMfaService _mfaService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IMfaService mfaService,
        ILogger<IdentityService> logger,
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _mfaService = mfaService;
        _logger = logger;
        _configuration = configuration;
        _roleManager = roleManager;
    }

    public async Task<ApplicationUser> CreateUser(CreateUserInput input)
    {
        var existingUser = await _userManager.FindByEmailAsync(input.Email);
        if (existingUser != null)
            throw new ValidationException("Email already registered");

        var user = new ApplicationUser
        {
            UserName = input.UserName,
            Email = input.Email,
            PhoneNumber = input.PhoneNumber,
            Created = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
            throw new ValidationException(result.Errors.First().Description);

        if (input.Roles?.Any() == true)
        {
            var roleResult = await _userManager.AddToRolesAsync(user, input.Roles);
            if (!roleResult.Succeeded)
                _logger.LogWarning("Failed to assign roles to user {UserId}", user.Id);
        }

        _logger.LogInformation("Created new user {UserId}", user.Id);
        return user;
    }

    public async Task<ApplicationUser> UpdateUser(UpdateUserInput input)
    {
        var user = await _userManager.FindByIdAsync(input.Id)
                   ?? throw new NotFoundException($"User {input.Id} not found");

        user.Email = input.Email;
        user.PhoneNumber = input.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new ValidationException(result.Errors.First().Description);

        _logger.LogInformation("Updated user {UserId}", user.Id);
        return user;
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return users;
    }

    public async Task<ApplicationUser> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
                   ?? throw new NotFoundException($"User {id} not found");

        return user;
    }

    public async Task DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
                   ?? throw new NotFoundException($"User {id} not found");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new ValidationException(result.Errors.First().Description);

        _logger.LogInformation("Deleted user {UserId}", id);
    }

    public async Task<TokenResponse> RefreshToken(string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(refreshToken);
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new NotFoundException($"User {userId} not found");

        var roles = await _userManager.GetRolesAsync(user);
        var newToken = GenerateToken(user, roles);

        _logger.LogInformation("Refreshed token for user {UserId}", user.Id);
        return newToken;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null) throw new AuthenticationException();

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) throw new AuthenticationException();

        return await _tokenService.GenerateTokenAsync(user);
    }

    private TokenResponse GenerateToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? 
                                   throw new InvalidOperationException("JWT key not configured")));
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

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? 
                                       throw new InvalidOperationException("JWT key not configured"))),
            ValidateLifetime = false // we are only validating the token signature here
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }

    public async Task<TokenResponse> Login(LoginInput input)
    {
        var user = await _userManager.FindByNameAsync(input.Username)
            ?? throw new AuthenticationException("Invalid credentials");

        var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, true);
        if (!result.Succeeded)
            throw new AuthenticationException("Invalid credentials");

        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            if (string.IsNullOrEmpty(input.MfaCode))
                throw new MfaRequiredException();

            var isValidMfaCode = await ValidateMfaCodeAsync(user.Id, input.MfaCode);
            if (!isValidMfaCode)
                throw new AuthenticationException("Invalid MFA code");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return await _tokenService.GenerateTokens(user.Id, roles);
    }

    public async Task<bool> EnableMfaAsync(string userId, string verificationCode)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User {userId} not found");

        if (await _userManager.GetTwoFactorEnabledAsync(user))
            throw new InvalidOperationException("MFA is already enabled");

        var secretKey = await _mfaService.GenerateSecretKey();
        var isValid = await _mfaService.ValidateCode(secretKey, verificationCode);

        if (!isValid)
            return false;

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        await _userManager.SetAuthenticationTokenAsync(
            user, "MFA", "SecretKey", secretKey);

        _logger.LogInformation("MFA enabled for user {UserId}", userId);
        return true;
    }

    public async Task<bool> DisableMfaAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User {userId} not found");

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
            return false;

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.RemoveAuthenticationTokenAsync(
            user, "MFA", "SecretKey");

        _logger.LogInformation("MFA disabled for user {UserId}", userId);
        return true;
    }

    public async Task<bool> ValidateMfaCodeAsync(string userId, string verificationCode)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User {userId} not found");

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
            return false;

        var secretKey = await _userManager.GetAuthenticationTokenAsync(
            user, "MFA", "SecretKey");

        return await _mfaService.ValidateCode(secretKey, verificationCode);
    }

    public async Task<bool> AssignRoleAsync(string userId, string role)
    {
        var user = await GetUserById(userId);
        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to assign role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }

        return true;
    }

    public async Task<bool> RemoveRoleAsync(string userId, string role)
    {
        var user = await GetUserById(userId);
        var result = await _userManager.RemoveFromRoleAsync(user, role);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to remove role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }

        return true;
    }
}