using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tinker.Shared.DTOs.Auth;

namespace Tinker.Core.Security.Policies;

public class CustomAuthPolicyProvider(
    IOptions<AuthorizationOptions>    options,
    ILogger<CustomAuthPolicyProvider> logger)
    : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        try
        {
            // Check for cached policy first
            var policy = await _fallbackPolicyProvider.GetPolicyAsync(policyName);
            if (policy != null) return policy;

            var builder = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser();

            if (RoleConfiguration.Roles.TryGetValue(policyName, out var role))
            {
                builder.RequireRole(role.Name);

                if (role.RequiresMfa)
                    builder.AddRequirements(new MfaRequirement());

                foreach (var permission in role.Permissions)
                    builder.RequireClaim("Permission", permission);
            }

            return builder.Build();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating policy: {PolicyName}", policyName);
            throw;
        }
    }

    private AuthorizationPolicy CreateRxPolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Pharmacist", "PharmacyTech")
            .RequireClaim("RxLicense")
            .RequireClaim("TrainingComplete", "true")
            .AddRequirements(new MfaRequirement())
            .Build();
    }

    private AuthorizationPolicy CreateAdminPolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Admin")
            .RequireClaim("AdminLevel", "1", "2", "3")
            .AddRequirements(new MfaRequirement())
            .AddRequirements(new IpAddressRequirement(options.Value.AllowedIpRanges))
            .Build();
    }

    private AuthorizationPolicy CreateInventoryPolicy(string policyName)
    {
        var builder = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser();

        if (policyName.Contains("Write"))
            builder.RequireRole("InventoryManager", "Admin")
                .RequireClaim("InventoryAccess", "Write")
                .AddRequirements(new BusinessHoursRequirement());
        else
            builder.RequireRole("InventoryManager", "Admin", "Staff")
                .RequireClaim("InventoryAccess", "Read");

        return builder.Build();
    }
}