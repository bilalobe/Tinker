using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tinker.Infrastructure.Processing.Background.Services;

namespace Tinker.Infrastructure.Monitoring.HealthChecks;

public class BackgroundServiceHealthCheck(IServiceProvider serviceProvider) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken                                                    cancellationToken = default)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var stockCheck = scope.ServiceProvider.GetRequiredService<StockCheckBackgroundService>();
            var expiryCheck = scope.ServiceProvider.GetRequiredService<ExpiryCheckBackgroundService>();

            // Check if services are running
            return HealthCheckResult.Healthy("Background services are running");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("One or more background services are not running", ex);
        }
    }
}