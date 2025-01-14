using Microsoft.AspNetCore.Identity;
using Tinker.Infrastructure.Abstractions.Identity;
using Tinker.Infrastructure.Identity.Core.Models;

namespace Tinker.Infrastructure.Identity.Stores;

public class UserStore : IUserStore
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserStore(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser> FindByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException($"User with ID {userId} not found");
        }
        return user;
    }

    public async Task<bool> EnableMfaAsync(string userId)
    {
        var user = await FindByIdAsync(userId);
        if (await _userManager.GetTwoFactorEnabledAsync(user))
        {
            return false; // MFA already enabled
        }

        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
        var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Failed to enable MFA");
        }

        // Optionally, send the token to the user via email or SMS
        // await _emailService.SendMfaTokenAsync(user.Email, token);

        return true;
    }
}