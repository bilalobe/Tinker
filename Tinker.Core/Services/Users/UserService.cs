using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Users.Entities;
using Tinker.Core.Domain.Users.Extensions;
using Tinker.Core.Domain.Users.Repositories;
using Tinker.Core.Domain.Users.ValueObjects;
using Tinker.Core.Services.Users.Interfaces;
using Tinker.Shared.DTOs.Users;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Users;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserByIdAsync(UserId id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user?.ToDto();
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.ToDto());
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto input)
    {
        if (await _userRepository.ExistsByEmailAsync(input.Email))
            throw new ValidationException("Email already exists");

        if (await _userRepository.ExistsByUserNameAsync(input.UserName))
            throw new ValidationException("Username already exists");

        var user = new User(UserId.New(), input.UserName, input.Email);
        user.UpdateProfile(input.PhoneNumber);

        await _userRepository.AddAsync(user);
        _logger.LogInformation("Created new user {UserId}", user.Id);

        return user.ToDto();
    }

    public async Task<bool> EnableMfaAsync(UserId userId, string verificationCode)
    {
        var user = await _userRepository.GetByIdAsync(userId)
                   ?? throw new NotFoundException($"User {userId} not found");

        // TODO: Implement proper MFA verification
        if (verificationCode == "123456")
        {
            user.EnableMfa("secret");
            await _userRepository.UpdateAsync(user);
            return true;
        }

        return false;
    }

    public Task UpdateUserAsync(UpdateUserDto input)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(UserId id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DisableMfaAsync(UserId userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateMfaCodeAsync(UserId userId, string verificationCode)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserEntityByIdAsync(UserId id)
    {
        throw new NotImplementedException();
    }

    // Implement other interface methods...
}