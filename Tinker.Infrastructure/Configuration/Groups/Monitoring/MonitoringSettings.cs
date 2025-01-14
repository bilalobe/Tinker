namespace Tinker.Infrastructure.Configuration.Groups.Monitoring;

using FluentValidation;
using Microsoft.Extensions.Logging;
using Tinker.Infrastructure.Configuration.Base;

public class MonitoringSettings : ISettings
{
    public MetricsSettings Metrics { get; init; } = new();
    public HealthCheckSettings HealthCheck { get; init; } = new();
    public LoggingSettings Logging { get; init; } = new();
    public bool EnableDetailedErrors { get; init; }

    public void Validate() => 
        new MonitoringSettingsValidator().ValidateSettings(this, "Monitoring settings");
}

public class MetricsSettings
{
    public bool Enabled { get; init; }
    public TimeSpan CollectionInterval { get; init; }
    public int RetentionDays { get; init; } = 30;
}

public class HealthCheckSettings
{
    public string Endpoint { get; init; } = "/health";
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(5);
    public long MemoryThresholdBytes { get; init; } = 1024L * 1024L * 1024L;
    public int MaxFailedAttempts { get; init; } = 3;
}

public class LoggingSettings
{
    public string LogPath { get; init; } = "logs/tinker-.log";
    public int RetentionDays { get; init; } = 90;
    public bool EnableStructuredLogging { get; init; } = true;
    public LogLevel MinimumLevel { get; init; } = LogLevel.Information;
}

public class MonitoringSettingsValidator : SettingsValidatorBase<MonitoringSettings>
{
    public MonitoringSettingsValidator()
    {
        RuleFor(x => x.Metrics).NotNull().SetValidator(new MetricsSettingsValidator());
        RuleFor(x => x.HealthCheck).NotNull().SetValidator(new HealthCheckSettingsValidator());
        RuleFor(x => x.Logging).NotNull().SetValidator(new LoggingSettingsValidator());
    }
}

public class MetricsSettingsValidator : AbstractValidator<MetricsSettings>
{
    public MetricsSettingsValidator()
    {
        When(x => x.Enabled, () =>
        {
            RuleFor(x => x.CollectionInterval)
                .InclusiveBetween(TimeSpan.FromSeconds(1), TimeSpan.FromHours(1));
            RuleFor(x => x.RetentionDays)
                .InclusiveBetween(1, 365);
        });
    }
}

public class HealthCheckSettingsValidator : AbstractValidator<HealthCheckSettings>
{
    public HealthCheckSettingsValidator()
    {
        RuleFor(x => x.Endpoint).NotEmpty().Matches("^/[a-zA-Z0-9/-]+$");
        RuleFor(x => x.Timeout).InclusiveBetween(TimeSpan.FromSeconds(1), TimeSpan.FromMinutes(5));
        RuleFor(x => x.MemoryThresholdBytes).GreaterThan(0);
        RuleFor(x => x.MaxFailedAttempts).InclusiveBetween(1, 10);
    }
}

public class LoggingSettingsValidator : AbstractValidator<LoggingSettings>
{
    public LoggingSettingsValidator()
    {
        RuleFor(x => x.LogPath)
            .NotEmpty()
            .Must(path => !Path.IsPathRooted(path) || Directory.Exists(Path.GetDirectoryName(path)));
        RuleFor(x => x.RetentionDays).InclusiveBetween(1, 365);
        RuleFor(x => x.MinimumLevel).IsInEnum();
    }
}