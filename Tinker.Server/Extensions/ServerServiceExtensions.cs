namespace Tinker.Server.Extensions;

public static class ServerServiceExtensions
{
    public static IServiceCollection AddServerInfrastructure(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        return services
            .AddDatabaseServices(configuration)
            .AddCacheServices(configuration)
            .AddAuthServices(configuration)
            .AddMonitoringServices(configuration)
            .AddBackgroundServices();
    }
}