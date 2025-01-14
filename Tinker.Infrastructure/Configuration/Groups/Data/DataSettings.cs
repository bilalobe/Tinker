namespace Tinker.Infrastructure.Configuration.Groups.Data;

using FluentValidation;
using Tinker.Infrastructure.Configuration.Base;

public class DataSettings : ISettings
{
    public required DatabaseSettings Database { get; init; }
    public required CacheSettings Cache { get; init; }

    public void Validate() => 
        new DataSettingsValidator().ValidateSettings(this, "Data settings");
}

public class DatabaseSettings : ISettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public int MaxRetryCount { get; init; }
    public int CommandTimeout { get; init; } = 30;
    public bool EnableDetailedErrors { get; init; }
    public bool EnableSensitiveDataLogging { get; init; }

    public void Validate() => 
        new DatabaseSettingsValidator().ValidateSettings(this, "Database settings");
}

public class CacheSettings : ISettings
{
    public bool Enabled { get; init; }
    public string ConnectionString { get; init; } = string.Empty;
    public string InstanceName { get; init; } = string.Empty;
    public TimeSpan DefaultExpiry { get; init; } = TimeSpan.FromMinutes(30);
    public int CompressionThreshold { get; init; } = 1024;

    public void Validate() => 
        new CacheSettingsValidator().ValidateSettings(this, "Cache settings");
}

public class DataSettingsValidator : SettingsValidatorBase<DataSettings>
{
    public DataSettingsValidator()
    {
        RuleFor(x => x.Database).NotNull().SetValidator(new DatabaseSettingsValidator());
        RuleFor(x => x.Cache).NotNull().SetValidator(new CacheSettingsValidator());
    }
}

public class DatabaseSettingsValidator : SettingsValidatorBase<DatabaseSettings>
{
    public DatabaseSettingsValidator()
    {
        RuleFor(x => x.ConnectionString).NotEmpty().MaximumLength(500);
        RuleFor(x => x.CommandTimeout).InclusiveBetween(1, 3600);
        RuleFor(x => x.MaxRetryCount).InclusiveBetween(0, 10);
        When(x => x.EnableSensitiveDataLogging, () =>
        {
            RuleFor(x => x.EnableDetailedErrors).Equal(true)
                .WithMessage("DetailedErrors must be enabled when SensitiveDataLogging is enabled");
        });
    }
}

public class CacheSettingsValidator : SettingsValidatorBase<CacheSettings>
{
    public CacheSettingsValidator()
    {
        When(x => x.Enabled, () =>
        {
            RuleFor(x => x.ConnectionString).NotEmpty().MaximumLength(500);
            RuleFor(x => x.InstanceName).NotEmpty().MaximumLength(50).Matches("^[a-zA-Z0-9-_]+$");
        });
        RuleFor(x => x.DefaultExpiry).InclusiveBetween(TimeSpan.FromSeconds(1), TimeSpan.FromDays(30));
        RuleFor(x => x.CompressionThreshold).InclusiveBetween(64, 1024 * 1024);
    }
}