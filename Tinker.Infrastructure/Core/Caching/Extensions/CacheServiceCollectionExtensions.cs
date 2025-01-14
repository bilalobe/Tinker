using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using StackExchange.Redis;
using Tinker.Infrastructure.Core.Caching.Interfaces;
using Tinker.Infrastructure.Core.Caching.Services;

namespace Tinker.Infrastructure.Core.Caching.Extensions;

public static class CacheServiceCollectionExtensions
{
    public static IServiceCollection AddResilientCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var config = configuration
            .GetSection("Cache")
            .Get<CacheConfiguration>();

        services.AddSingleton(config);

        services.AddResilienceHub("Cache", builder =>
        {
            builder.AddPipeline("cache", pipeline =>
            {
                pipeline
                    .AddRetry(new()
                    {
                        MaxRetryAttempts = config.RetryCount,
                        BackoffType = DelayBackoffType.Exponential,
                        Delay = config.RetryDelay
                    })
                    .AddCircuitBreaker(new()
                    {
                        FailureRatio = 0.3,
                        SamplingDuration = TimeSpan.FromSeconds(30),
                        MinimumThroughput = 10,
                        BreakDuration = TimeSpan.FromSeconds(30)
                    });
            });
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.ConnectionString;
            options.InstanceName = config.InstanceName;
        });

        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(config.ConnectionString));

        services.AddScoped<ICacheService, ResilientCacheService>();

        return services;
    }
}