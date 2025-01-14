using Tinker.Core.Domain.Users.Entities;
using Tinker.Core.Domain.Users.ValueObjects;
using Tinker.Shared.DTOs.Users;

namespace Tinker.Core.Services.Users.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(UserId id);
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserDto input);
    Task UpdateUserAsync(UpdateUserDto          input);
    Task DeleteUserAsync(UserId                 id);
    Task<bool> EnableMfaAsync(UserId            userId, string verificationCode);
    Task<bool> DisableMfaAsync(UserId           userId);
    Task<bool> ValidateMfaCodeAsync(UserId      userId, string verificationCode);
    Task<User?> GetUserEntityByIdAsync(UserId   id);
}