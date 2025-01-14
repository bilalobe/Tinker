namespace Tinker.Infrastructure.Configuration.Base;

using Microsoft.Extensions.Configuration;
using System;

public class ConfigurationManager : ISettingsFactory
{
    private readonly IConfiguration _configuration;
    private readonly IDictionary<Type, object> _settingsCache;

    public ConfigurationManager(IConfiguration configuration)
    {
        _configuration = configuration;
        _settingsCache = new Dictionary<Type, object>();
    }

    public T CreateSettings<T>() where T : ISettings
    {
        var type = typeof(T);
        if (_settingsCache.TryGetValue(type, out var cached))
            return (T)cached;

        var sectionName = GetSectionName<T>();
        var settings = _configuration.GetSection(sectionName).Get<T>() 
            ?? throw new InvalidOperationException($"Settings not found for section: {sectionName}");
        
        settings.Validate();
        _settingsCache[type] = settings;
        
        return settings;
    }

    private static string GetSectionName<T>()
    {
        var type = typeof(T);
        return type.Name.Replace("Settings", string.Empty);
    }
}
