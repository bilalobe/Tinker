using FluentValidation;
using Tinker.Infrastructure.Configuration.Base;

namespace Tinker.Infrastructure.Configuration.Groups.Background;

public class BackgroundSettings : ISettings
{
    public JobSettings Jobs { get; init; } = new();
    public ThresholdSettings Thresholds { get; init; } = new();

    public void Validate() =>
        new BackgroundSettingsValidator().ValidateSettings(this, "Background settings");
}

public class JobSettings
{
    public int StockCheckIntervalMinutes { get; init; } = 60;
    public int ExpiryCheckIntervalMinutes { get; init; } = 120;
    public int MaintenanceIntervalHours { get; init; } = 24;
    public bool EnableAutoRetry { get; init; } = true;
    public int MaxRetryAttempts { get; init; } = 3;
}

public class ThresholdSettings
{
    public int MinimumStockThreshold { get; init; } = 10;
    public int ExpiryWarningDays { get; init; } = 30;
    public int LowStockWarningLevel { get; init; } = 20;
    public int CriticalStockLevel { get; init; } = 5;
}

public class BackgroundSettingsValidator : SettingsValidatorBase<BackgroundSettings>
{
    public BackgroundSettingsValidator()
    {
        RuleFor(x => x.Jobs).SetValidator(new JobSettingsValidator());
        RuleFor(x => x.Thresholds).SetValidator(new ThresholdSettingsValidator());
    }
}

public class JobSettingsValidator : AbstractValidator<JobSettings>
{
    public JobSettingsValidator()
    {
        RuleFor(x => x.StockCheckIntervalMinutes).InclusiveBetween(1, 1440)
            .WithMessage("Stock check interval must be between 1 and 1440 minutes");
        RuleFor(x => x.ExpiryCheckIntervalMinutes).InclusiveBetween(1, 1440)
            .WithMessage("Expiry check interval must be between 1 and 1440 minutes");
        RuleFor(x => x.MaintenanceIntervalHours).InclusiveBetween(1, 168)
            .WithMessage("Maintenance interval must be between 1 and 168 hours");
        RuleFor(x => x.MaxRetryAttempts).InclusiveBetween(0, 10)
            .WithMessage("Max retry attempts must be between 0 and 10");
    }
}

public class ThresholdSettingsValidator : AbstractValidator<ThresholdSettings>
{
    public ThresholdSettingsValidator()
    {
        RuleFor(x => x.MinimumStockThreshold).GreaterThan(0)
            .WithMessage("Minimum stock threshold must be greater than 0");
        RuleFor(x => x.ExpiryWarningDays).InclusiveBetween(1, 365)
            .WithMessage("Expiry warning days must be between 1 and 365");
        RuleFor(x => x.LowStockWarningLevel).GreaterThan(x => x.CriticalStockLevel)
            .WithMessage("Low stock warning level must be greater than critical stock level");
        RuleFor(x => x.CriticalStockLevel).GreaterThan(0)
            .WithMessage("Critical stock level must be greater than 0");
    }
}
