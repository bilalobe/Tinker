using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Tinker.Infrastructure.Security.Authorization.Handlers;

public class RoleAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext       context,
        OperationAuthorizationRequirement requirement)
    {
        var userRole = context.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        if (userRole != null &&
            RoleConfiguration.Roles.TryGetValue(userRole, out var role) &&
            role.AllowedOperations.Contains(requirement.Name))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}