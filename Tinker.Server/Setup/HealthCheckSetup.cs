using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Tinker.Infrastructure.Core.Data.Context;
using Tinker.Infrastructure.Monitoring.Health.Checks;
using Tinker.Infrastructure.Monitoring.HealthChecks;

namespace Tinker.Server.Setup;

public static class HealthCheckSetup
{
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        return services.AddHealthChecks()
            // Infrastructure
            .AddDbContextCheck<ApplicationDbContext>()
            .AddRedis(configuration.GetConnectionString("Redis"))
            
            // Custom Checks
            .AddCheck<DatabaseHealthCheck>("database")
            .AddCheck<MemoryHealthCheck>("memory")
            .AddCheck<SecurityHealthCheck>("security")
            .AddCheck<BackgroundJobHealthCheck>("background-jobs")
            .AddCheck<BackgroundServiceHealthCheck>("BackgroundServices")
            
            // System Checks
            .AddDiskStorageHealthCheck(setup =>
            {
                setup.AddDrive("C:\\", 1024); // 1GB minimum
            })
            .AddProcessAllocatedMemoryHealthCheck(512) // 512MB maximum
            .AddProcessHealthCheck("System", p => p.Length < 100); // Max 100 processes
    }

    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(x => new
                    {
                        name = x.Key,
                        status = x.Value.Status.ToString(),
                        description = x.Value.Description,
                        duration = x.Value.Duration
                    })
                };
                await JsonSerializer.SerializeAsync(context.Response.Body, response);
            },
            AllowCachingResponses = false,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        });

        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
            options.AddCustomStylesheet("wwwroot/css/health-ui.css");
        });

        return app;
    }
}