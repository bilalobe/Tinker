using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tinker.Infrastructure.Configuration.Groups.Data;

namespace Tinker.Infrastructure.Configuration.Setup;

public static class CacheSetup
{
    public static IServiceCollection AddCacheSetup(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("Infrastructure:Data:Cache")
            .Get<CacheSettings>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = settings.ConnectionString;
            options.InstanceName = settings.InstanceName;
        });

        return services;
    }
}