namespace Tinker.Infrastructure.Configuration.Groups.Auth;

using FluentValidation;
using Tinker.Infrastructure.Configuration.Base;

public class AuthSettings : ISettings
{
    public JwtSettings Jwt { get; init; } = new();
    public PasswordSettings Password { get; init; } = new();
    public LockoutSettings Lockout { get; init; } = new();
    public SignInSettings SignIn { get; init; } = new();

    public void Validate()
    {
        var validator = new AuthSettingsValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new FluentValidation.ValidationException("Auth settings validation failed", result.Errors);
    }
}

public class JwtSettings
{
    public string SecretKey { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
    public int ClockSkewMinutes { get; init; } = 2;
    public bool ValidateIssuer { get; init; } = true;
    public bool ValidateAudience { get; init; } = true;
    public bool ValidateLifetime { get; init; } = true;
    public bool ValidateIssuerSigningKey { get; init; } = true;
}

public class PasswordSettings
{
    public int RequiredLength { get; init; } = 12;
    public bool RequireDigit { get; init; } = true;
    public bool RequireLowercase { get; init; } = true;
    public bool RequireUppercase { get; init; } = true;
    public bool RequireNonAlphanumeric { get; init; } = true;
}

public class LockoutSettings
{
    public int DefaultLockoutTimeSpanMinutes { get; init; } = 30;
    public int MaxFailedAccessAttempts { get; init; } = 3;
}

public class SignInSettings
{
    public bool RequireConfirmedEmail { get; init; } = true;
    public bool RequireConfirmedAccount { get; init; } = true;
}

public class AuthSettingsValidator : AbstractValidator<AuthSettings>
{
    public AuthSettingsValidator()
    {
        RuleFor(x => x.Jwt.SecretKey).NotEmpty().MinimumLength(32);
        RuleFor(x => x.Jwt.Issuer).NotEmpty();
        RuleFor(x => x.Jwt.Audience).NotEmpty();
        RuleFor(x => x.Jwt.AccessTokenExpirationMinutes).InclusiveBetween(1, 60);
        RuleFor(x => x.Jwt.RefreshTokenExpirationDays).InclusiveBetween(1, 30);
        RuleFor(x => x.Jwt.ClockSkewMinutes).InclusiveBetween(0, 5);

        RuleFor(x => x.Password.RequiredLength).InclusiveBetween(8, 128);

        RuleFor(x => x.Lockout.DefaultLockoutTimeSpanMinutes).InclusiveBetween(1, 1440);
        RuleFor(x => x.Lockout.MaxFailedAccessAttempts).InclusiveBetween(1, 10);
    }
}
