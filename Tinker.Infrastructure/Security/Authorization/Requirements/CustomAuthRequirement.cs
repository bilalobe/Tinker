using Microsoft.AspNetCore.Authorization;

namespace Tinker.Infrastructure.Security.Authorization.Requirements;

public class CustomAuthRequirement(string policyName) : IAuthorizationRequirement
{
    public string PolicyName { get; } = policyName;
}