namespace Tinker.Infrastructure.Configuration.Base;

public interface ISettingsFactory
{
    T CreateSettings<T>() where T : ISettings;
}