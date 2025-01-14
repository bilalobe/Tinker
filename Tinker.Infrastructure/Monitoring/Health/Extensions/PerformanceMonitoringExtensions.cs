using Microsoft.Extensions.DependencyInjection;
using Tinker.Infrastructure.Core.Data.Interfaces;
using Tinker.Infrastructure.Monitoring.Metrics.Collectors;

namespace Tinker.Infrastructure.Monitoring.Health.Extensions;

public static class PerformanceMonitoringExtensions
{
    public static IServiceCollection AddPerformanceMonitoring(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        services.AddMetrics();
        services.AddPerformanceTracking();

        // Register performance monitors
        services.AddHostedService<PerformanceMetricsCollector>();

        return services;
    }
}