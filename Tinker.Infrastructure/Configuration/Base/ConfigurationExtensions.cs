namespace Tinker.Infrastructure.Configuration.Base;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddSettings<T>(
        this IServiceCollection services,
        IConfiguration configuration) where T : class, ISettings
    {
        var settings = configuration.GetSection(typeof(T).Name).Get<T>();
        if (settings == null)
            throw new InvalidOperationException($"Settings not found for {typeof(T).Name}");

        settings.Validate();
        services.AddSingleton(settings);
        
        return services;
    }

    public static IServiceCollection AddSettingsFactory(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ISettingsFactory>(new ConfigurationManager(configuration));
        return services;
    }
}
