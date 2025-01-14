using Microsoft.Extensions.Logging;
using Tinker.Shared.Models.Responses;

namespace Tinker.Infrastructure.Security.Authorization.Handlers;

public class EnableMfaCommandHandler(IUserService userService, ILogger<EnableMfaCommandHandler> logger)
    : IRequestHandler<EnableMfaCommand, MfaResult>
{
    private readonly IUserService _userService = userService;

    public async Task<MfaResult> Handle(EnableMfaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var isEnabled = await _userService.EnableMfaAsync(request.UserId, request.VerificationCode);
            if (!isEnabled)
            {
                logger.LogWarning("Failed to enable MFA for user {UserId}", request.UserId);
                return MfaResult.Failed("Invalid verification code");
            }

            logger.LogInformation("MFA enabled successfully for user {UserId}", request.UserId);
            return MfaResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enabling MFA for user {UserId}", request.UserId);
            return MfaResult.Failed("An unexpected error occurred");
        }
    }
}