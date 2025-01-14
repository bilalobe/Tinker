using System.Text.Json;
using AspNetCoreRateLimit;
using Tinker.Infrastructure.Monitoring.Health.Checks;
using Tinker.Infrastructure.Monitoring.Metrics.Collectors;
using Tinker.Server.Configuration.DependencyInjection;

namespace Tinker.Server.Setup;

/// <summary>
/// Provides extension methods for configuring application services
/// </summary>
public static class ServicesSetup
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var settings = builder.Configuration
            .GetSection("AppSettings")
            .Get<AppSettings>() ?? throw new InvalidOperationException("AppSettings not configured");

        // API Services
        builder.Services
            .AddGraphQLServices()
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        // Health Checks
        builder.Services
            .AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck<MemoryHealthCheck>("memory")
            .AddCheck<SecurityHealthCheck>("security")
            .AddCheck<BackgroundJobHealthCheck>("background-jobs");

        // Rate Limiting
        builder.Services.AddRateLimiting(options =>
        {
            options.GeneralRules = new[]
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1s",
                    Limit = 10
                }
            };
        });

        // Monitoring
        builder.Services
            .AddMonitoringServices(settings)
            .AddPerformanceMonitoring(builder.Configuration)
            .AddHostedService<CustomMetricsCollector>();

        // Caching
        builder.Services
            .AddResponseCaching()
            .AddOutputCache(options =>
            {
                options.AddBasePolicy(builder =>
                    builder.Cache());
                options.AddPolicy("Expire30", builder =>
                    builder.Expire(TimeSpan.FromSeconds(30)));
            });

        return builder;
    }
}