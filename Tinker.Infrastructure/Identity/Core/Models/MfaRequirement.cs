using Microsoft.AspNetCore.Authorization;

namespace Tinker.Infrastructure.Identity.Core.Models;
public class MfaRequirement : IAuthorizationRequirement
{
    public bool RequiredForAllUsers { get; }
    public bool RequiredForAdmins { get; }
    public TimeSpan GracePeriod { get; }

    public MfaRequirement(bool requiredForAll = false, bool requiredForAdmins = true)
    {
        RequiredForAllUsers = requiredForAll;
        RequiredForAdmins = requiredForAdmins;
        GracePeriod = TimeSpan.FromDays(7);
    }
}