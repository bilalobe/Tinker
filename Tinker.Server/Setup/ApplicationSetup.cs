
using Tinker.Infrastructure;
using Tinker.Infrastructure.Configuration.Setup;
using Tinker.Server.Configuration.DependencyInjection;

namespace Tinker.Server.Setup;

public static class ApplicationSetup
{
    public static WebApplicationBuilder ConfigureApplication(this WebApplicationBuilder builder)
    {
        // Load and validate configuration
        var settings = builder.Configuration
            .GetSection("AppSettings")
            .Get<AppSettings>() ?? throw new InvalidOperationException("AppSettings not configured");

        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

        // Infrastructure Layer
        builder.Services
            .AddDatabaseSetup(builder.Configuration)
            .AddCacheSetup(builder.Configuration)
            .AddAuthSetup(builder.Configuration)
            .AddMonitoringSetup(builder.Configuration);

        // Application Layer
        builder.Services
            .AddApplicationServices()
            .AddBackgroundServices();

        // API Layer
        builder.Services
            .AddGraphQLServices()
            .AddHealthChecks()
            .AddSwaggerServices()
            .AddServerServices();

        return builder;
    }
}