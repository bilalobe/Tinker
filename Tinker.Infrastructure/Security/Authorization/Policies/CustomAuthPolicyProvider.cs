using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tinker.Infrastructure.Security.Authorization.Policies;

public class CustomAuthPolicyProvider(
    IOptions<AuthorizationOptions>    options,
    ILogger<CustomAuthPolicyProvider> logger)
    : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);
    private readonly IOptions<AuthorizationOptions> _options = options;

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

            // Handle different policy types
            return policyName switch
                   {
                       var name when name.StartsWith("Rx") =>
                           CreateRxPolicy(),

                       var name when name.StartsWith("Admin") =>
                           CreateAdminPolicy(),

                       var name when name.StartsWith("Inventory") =>
                           CreateInventoryPolicy(name),

                       _ => await _fallbackPolicyProvider.GetPolicyAsync(policyName)
                   };
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
            .Build();
    }

    private AuthorizationPolicy CreateAdminPolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("Admin")
            .Build();
    }

    private AuthorizationPolicy CreateInventoryPolicy(string policyName)
    {
        var builder = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser();

        if (policyName.Contains("Write"))
            builder.RequireRole("InventoryManager", "Admin");
        else
            builder.RequireRole("InventoryManager", "Admin", "Staff");

        return builder.Build();
    }
}