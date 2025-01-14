using Microsoft.AspNetCore.Authorization;

namespace Tinker.Core.Security.Requirements;

public class CustomAuthRequirementHandler : AuthorizationHandler<CustomAuthRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        CustomAuthRequirement       requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
            return Task.CompletedTask;

        if (context.User.HasClaim(c => c.Type == "CustomClaim" && c.Value == requirement.PolicyName) &&
            context.User.HasClaim(c => c.Type == "SecurityLevel" && int.Parse(c.Value) >= 2))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}