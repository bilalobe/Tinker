namespace Tinker.Infrastructure.Configuration.Groups.Security;

using FluentValidation;
using Tinker.Infrastructure.Configuration.Base;

public class SecuritySettings : ISettings
{
    public AuthenticationSettings Authentication { get; init; } = new();
    public PasswordSettings Password { get; init; } = new();
    public SessionSettings Session { get; init; } = new();
    public AuditSettings Audit { get; init; } = new();
    public MfaSettings Mfa { get; init; } = new();

    public void Validate() => 
        new SecuritySettingsValidator().ValidateSettings(this, "Security settings");
}

public class AuthenticationSettings
{
    public int MaxFailedAccessAttempts { get; init; } = 3;
    public int LockoutTimeSpanInMinutes { get; init; } = 30;
    public bool RequireUniqueEmail { get; init; } = true;
    public bool EnablePasswordlessLogin { get; init; } = false;
    public bool EnforceHttpsEverywhere { get; init; } = true;
    public int FailedLoginDelaySeconds { get; init; } = 2;
}

public class PasswordSettings
{
    public int MinimumLength { get; init; } = 12;
    public bool RequireUppercase { get; init; } = true;
    public bool RequireLowercase { get; init; } = true;
    public bool RequireDigits { get; init; } = true;
    public bool RequireSpecialCharacters { get; init; } = true;
    public bool PreventPasswordReuse { get; init; } = true;
    public int PasswordHistoryCount { get; init; } = 5;
    public int ExpirationDays { get; init; } = 60;
}

public class SessionSettings
{
    public int TimeoutMinutes { get; init; } = 15;
    public int MaxConcurrentSessions { get; init; } = 3;
    public bool PreventConcurrentLogins { get; init; } = true;
    public bool AllowRememberMe { get; init; } = false;
    public int TokenValidityMinutes { get; init; } = 30;
    public int RefreshTokenValidityDays { get; init; } = 7;
    public int TokenRefreshWindowInMinutes { get; init; } = 5;
}

public class AuditSettings
{
    public bool EnableSecurityAuditLog { get; init; } = true;
    public int RetentionDays { get; init; } = 90;
    public bool EnableSecurityScanning { get; init; } = true;
    public int SecurityScanIntervalHours { get; init; } = 24;
}

public class MfaSettings
{
    public bool RequireMfa { get; init; } = true;
    public bool EnforceMfaForAdmins { get; init; } = true;
    public int TimeWindowInSeconds { get; init; } = 180;
}

public class SecuritySettingsValidator : SettingsValidatorBase<SecuritySettings>
{
    public SecuritySettingsValidator()
    {
        RuleFor(x => x.Authentication).SetValidator(new AuthenticationSettingsValidator());
        RuleFor(x => x.Password).SetValidator(new PasswordSettingsValidator());
        RuleFor(x => x.Session).SetValidator(new SessionSettingsValidator());
        RuleFor(x => x.Audit).SetValidator(new AuditSettingsValidator());
        RuleFor(x => x.Mfa).SetValidator(new MfaSettingsValidator());
    }
}

public class AuthenticationSettingsValidator : AbstractValidator<AuthenticationSettings>
{
    public AuthenticationSettingsValidator()
    {
        RuleFor(x => x.MaxFailedAccessAttempts).InclusiveBetween(1, 10);
        RuleFor(x => x.LockoutTimeSpanInMinutes).InclusiveBetween(5, 1440);
        RuleFor(x => x.FailedLoginDelaySeconds).InclusiveBetween(1, 30);
    }
}

public class PasswordSettingsValidator : AbstractValidator<PasswordSettings>
{
    public PasswordSettingsValidator()
    {
        RuleFor(x => x.MinimumLength).InclusiveBetween(8, 128);
        RuleFor(x => x.PasswordHistoryCount).InclusiveBetween(1, 24);
        RuleFor(x => x.ExpirationDays).InclusiveBetween(1, 365);
    }
}

public class SessionSettingsValidator : AbstractValidator<SessionSettings>
{
    public SessionSettingsValidator()
    {
        RuleFor(x => x.TimeoutMinutes).InclusiveBetween(5, 480);
        RuleFor(x => x.MaxConcurrentSessions).InclusiveBetween(1, 10);
        RuleFor(x => x.TokenValidityMinutes).InclusiveBetween(5, 1440);
        RuleFor(x => x.RefreshTokenValidityDays).InclusiveBetween(1, 365);
        RuleFor(x => x.TokenRefreshWindowInMinutes).InclusiveBetween(1, 60);
    }
}

public class AuditSettingsValidator : AbstractValidator<AuditSettings>
{
    public AuditSettingsValidator()
    {
        RuleFor(x => x.RetentionDays).InclusiveBetween(30, 365);
        RuleFor(x => x.SecurityScanIntervalHours).InclusiveBetween(1, 168);
    }
}

public class MfaSettingsValidator : AbstractValidator<MfaSettings>
{
    public MfaSettingsValidator()
    {
        RuleFor(x => x.TimeWindowInSeconds).InclusiveBetween(30, 300);
    }
}