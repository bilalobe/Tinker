using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tinker.Infrastructure.Identity.Core.Interfaces;

namespace Tinker.Infrastructure.Security.Services;

public class HealthCheckService : IHealthCheck
{
    private readonly IDistributedCache _cache;
    private readonly ITokenService _tokenService;

    public HealthCheckService(IDistributedCache cache, ITokenService tokenService)
    {
        _cache = cache;
        _tokenService = tokenService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify token generation
            var testToken = await _tokenService.GenerateTestToken();
            if (string.IsNullOrEmpty(testToken))
                return HealthCheckResult.Degraded();

            // Verify cache
            await _cache.SetAsync("health_check", new byte[] { 1 }, cancellationToken);
            var result = await _cache.GetAsync("health_check", cancellationToken);
            if (result == null)
                return HealthCheckResult.Degraded();

            return HealthCheckResult.Healthy("Security service is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}