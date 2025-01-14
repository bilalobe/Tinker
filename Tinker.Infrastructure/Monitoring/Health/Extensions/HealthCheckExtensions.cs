using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tinker.Infrastructure.Monitoring.Health.Checks;
using Tinker.Infrastructure.Monitoring.Health.Models;

namespace Tinker.Infrastructure.Monitoring.Health.Extensions;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddDetailedHealthChecks(
        this IHealthChecksBuilder builder,
        HealthCheckOptions        options)
    {
        return builder
            .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "database" })
            .AddCacheHealthCheck()
            .AddProcessHealthCheck(options)
            .AddSystemMetrics(options)
            .AddBackgroundJobs()
            .AddExternalServices()
            .AddSecurityChecks();
    }

    private static IHealthChecksBuilder AddDatabaseHealthCheck(
        this IHealthChecksBuilder builder, HealthCheckOptions options)
    {
        return builder.AddCheck<DatabaseHealthCheck>("database", tags: new[] { "database" });
    }

    private static IHealthChecksBuilder AddSecurityChecks(
        this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<SecurityHealthCheck>("security", tags: new[] { "security" });
    }

    private static IHealthChecksBuilder AddBackgroundJobs(
        this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<BackgroundJobHealthCheck>(
            "background-jobs",
            tags: new[] { "background-jobs" });
    }

    private static IHealthChecksBuilder AddProcessHealthCheck(
        this IHealthChecksBuilder builder, HealthCheckOptions options)
    {
        return builder.AddCheck("process", () =>
        {
            using var process = Process.GetCurrentProcess();
            var health = new HealthCheckResult(
                HealthStatus.Healthy,
                "Process is healthy",
                data: new Dictionary<string, object>
                {
                    { "WorkingSet", process.WorkingSet64 },
                    { "ThreadCount", process.Threads.Count },
                    { "HandleCount", process.HandleCount },
                    { "ProcessorTime", process.TotalProcessorTime }
                });

            return Task.FromResult(health);
        }, new[] { "ready", "system" });
    }

    private static IHealthChecksBuilder AddSystemMetrics(
        this IHealthChecksBuilder builder, HealthCheckOptions options)
    {
        return builder.AddCheck("system", () =>
        {
            var driveInfo = DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .ToDictionary(
                    d => d.Name,
                    d => new
                    {
                        d.TotalSize,
                        AvailableSpace = d.AvailableFreeSpace
                    });

            var health = new HealthCheckResult(
                HealthStatus.Healthy,
                "System is healthy",
                data: new Dictionary<string, object>
                {
                    { "MachineName", Environment.MachineName },
                    { "OSVersion", Environment.OSVersion.ToString() },
                    { "Drives", driveInfo }
                });

            return Task.FromResult(health);
        }, new[] { "ready", "system" });
    }

    private static IHealthChecksBuilder AddPerformanceCounters(
        this IHealthChecksBuilder builder)
    {
        return builder.AddCheck("performance", () =>
        {
            var counters = new Dictionary<string, object>();

            if (OperatingSystem.IsWindows())
            {
                using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                using var memCounter = new PerformanceCounter("Memory", "Available MBytes");

                counters.Add("CpuUsage", cpuCounter.NextValue());
                counters.Add("AvailableMemory", memCounter.NextValue());
            }

            return Task.FromResult(new HealthCheckResult(
                HealthStatus.Healthy,
                "Performance counters collected",
                data: counters));
        });
    }

    private static IHealthChecksBuilder AddDependencyHealthChecks(
        this IHealthChecksBuilder builder)
    {
        return builder
            .AddUrlGroup(
                new Uri(options.ExternalServiceHealthCheckUri),
                name: "external-service",
                tags: new[] { "ready", "dependencies" })
            .AddRedis(
                "localhost:6379",
                name: "redis",
                tags: new[] { "ready", "dependencies" })
            .AddRabbitMQ(
                "amqp://localhost",
                name: "rabbitmq",
                tags: new[] { "ready", "dependencies" });
    }
}