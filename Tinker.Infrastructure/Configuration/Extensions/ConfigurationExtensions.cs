using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Tinker.Infrastructure.Configuration.Groups.Data;
using Tinker.Infrastructure.Configuration.Groups.Monitoring;
using Tinker.Infrastructure.Configuration.Groups.Security;

namespace Tinker.Infrastructure.Configuration.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddApplicationConfiguration(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        var appConfig = configuration.Get<AppConfiguration>()
            ?? throw new InvalidOperationException("Configuration not found");

        appConfig.Validate();

        services.AddSingleton(appConfig);
        services.Configure<DataSettings>(configuration.GetSection("Data"));
        services.Configure<SecuritySettings>(configuration.GetSection("Security"));
        services.Configure<MonitoringSettings>(configuration.GetSection("Monitoring"));
        services.Configure<LoggingSettings>(configuration.GetSection("Logging"));

        return services;
    }
}