using AspNetCoreRateLimit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Tinker.Server.Configuration.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServerServices(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());

        services.AddMemoryCache();
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }

    public static IApplicationBuilder UseServerServices(this IApplicationBuilder app)
    {
        app.UseHealthChecks("/health");
        app.UseIpRateLimiting();

        return app;
    }
}