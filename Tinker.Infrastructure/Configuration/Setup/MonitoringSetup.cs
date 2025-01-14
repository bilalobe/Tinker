using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Tinker.Infrastructure.Configuration.Groups.Monitoring;
using Tinker.Infrastructure.Core.Caching.Interfaces;
using Tinker.Infrastructure.Core.Data.Context;
using Tinker.Infrastructure.Monitoring.Core.Interfaces;
using Tinker.Infrastructure.Monitoring.Health.Extensions;
using Tinker.Infrastructure.Monitoring.Health.Models;
using Tinker.Infrastructure.Monitoring.Metrics.Collectors;
using Tinker.Infrastructure.Monitoring.Metrics.Services;
using Tinker.Infrastructure.Monitoring.Middleware;

namespace Tinker.Infrastructure.Configuration.Setup;

/// <summary>
/// Provides extension methods for configuring monitoring services
/// </summary>
public static class MonitoringSetup
{
    public static IServiceCollection AddMonitoringSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("Infrastructure:Monitoring")
            .Get<MonitoringSettings>() ?? throw new InvalidOperationException("Monitoring settings not configured");

        settings.Validate();
        
        AddMetricsServices(services, settings.Metrics);
        AddHealthCheckServices(services, settings.HealthCheck);
        AddLoggingServices(services, settings.Logging);

        // Application Insights
        services.AddApplicationInsightsTelemetry(options =>
        {
            options.EnableAdaptiveSampling = true;
            options.EnablePerformanceCounterCollectionModule = true;
            options.DeveloperMode = settings.EnableDetailedErrors;
        });

        services.AddSingleton<ITelemetryInitializer>(provider =>
        {
            return new CustomTelemetryInitializer
            {
                RoleName = "Tinker.Server",
                Environment = provider.GetRequiredService<IHostEnvironment>().EnvironmentName
            };
        });

        // Monitoring Services
        services.AddScoped<IPerformanceMonitor, PerformanceMonitor>();
        services.AddScoped<IHealthCheckService, HealthCheckService>();
        services.AddScoped<IMetricsReporter, MetricsReporter>();

        return services;
    }

    private static void AddMetricsServices(IServiceCollection services, MetricsSettings settings)
    {
        if (!settings.Enabled) return;

        services.AddSingleton<IMetricsService, MetricsService>();
        services.AddSingleton<ICacheMetrics, CacheMetrics>();
        services.AddHostedService<PerformanceMetricsCollector>();
    }

    private static void AddHealthCheckServices(IServiceCollection services, HealthCheckSettings settings)
    {
        services.AddHealthChecks()
            .AddDetailedHealthChecks(new HealthCheckOptions
            {
                MemoryThresholdBytes = settings.MemoryThresholdBytes,
                DatabaseTimeout = settings.Timeout,
                MaxFailedAttempts = settings.MaxFailedAttempts
            })
            .AddProcessHealthCheck()
            .AddSystemMetrics();
    }

    private static void AddLoggingServices(IServiceCollection services, LoggingSettings settings)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(settings.MinimumLevel)
            .WriteTo.Console()
            .WriteTo.File(
                settings.LogPath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: settings.RetentionDays);

        if (settings.EnableStructuredLogging)
        {
            logger.WriteTo.Seq("http://localhost:5341");
        }

        Log.Logger = logger.CreateLogger();

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });
    }

    public static IApplicationBuilder UseMonitoringSetup(
        this IApplicationBuilder app,
        MonitoringSettings settings)
    {
        // Health Check Endpoint
        app.UseHealthChecks(settings.HealthCheckEndpoint, new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        duration = entry.Value.Duration.TotalMilliseconds,
                        tags = entry.Value.Tags,
                        data = entry.Value.Data
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds
                };

                await JsonSerializer.SerializeAsync(
                    context.Response.Body,
                    response,
                    new JsonSerializerOptions { WriteIndented = true });
            },
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        // Performance Tracking
        app.UseMiddleware<PerformanceTrackingMiddleware>();

        return app;
    }

    public static IServiceCollection AddMonitoringServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Core metrics
        services.AddSingleton<IMetricsService, MetricsService>();
        services.AddSingleton<ICacheMetrics, CacheMetrics>();
        
        // Health checks
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>()
            .AddRedis(configuration.GetConnectionString("Redis"))
            .AddCheck<StorageHealthCheck>("storage");
            
        // Performance monitoring
        services.AddHostedService<PerformanceMetricsCollector>();
        
        return services;
    }
}