using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Tinker.Infrastructure.Identity.Services
{
    public class MfaHandler : IMfaHandler
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<MfaHandler> _logger;

        public MfaHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<MfaHandler> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<bool> EnableMfaAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            // Send token via email or SMS

            return true;
        }

        public async Task<bool> ValidateMfaTokenAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException("User not found");

            var result = await _userManager.VerifyTwoFactorTokenAsync(user, "Email", token);
            if (!result)
            {
                _logger.LogWarning("Invalid MFA token for user {UserId}", userId);
                return false;
            }

            return true;
        }
    }
}