using Microsoft.AspNetCore.Authorization;

namespace Tinker.Core.Security.Requirements;

public class CustomAuthRequirement(string policyName) : IAuthorizationRequirement
{
    public string PolicyName { get; } = policyName;
}