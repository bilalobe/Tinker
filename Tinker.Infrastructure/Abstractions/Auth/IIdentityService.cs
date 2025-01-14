using Tinker.Infrastructure.Identity.Core.Models;
using Tinker.Shared.Models.Auth;
using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Identity.Core.Interfaces;

public interface IIdentityService
{
    Task<ApplicationUser> CreateUser(CreateUserInput input);
    Task<TokenResponse> Login(LoginInput             input);
    Task<IEnumerable<ApplicationUser>> GetUsers();
    Task<ApplicationUser> GetUserById(string         id);
    Task<ApplicationUser> UpdateUser(UpdateUserInput input);
    Task DeleteUser(string                           id);
    Task<TokenResponse> RefreshToken(string          refreshToken);
    Task<bool> EnableMfaAsync(string                 userId, string verificationCode);
    Task<bool> DisableMfaAsync(string                userId);
    Task<bool> ValidateMfaCodeAsync(string           userId, string verificationCode);
}