using Tinker.Infrastructure.Processing.Background.Services;

namespace Tinker.Server.Configuration.DependencyInjection;

public static class BackgroundServiceConfig
{
    public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        services.AddHostedService<StockCheckBackgroundService>();
        services.AddHostedService<ExpiryCheckBackgroundService>();
        services.AddHostedService<TaskSchedulerService>();

        return services;
    }
}