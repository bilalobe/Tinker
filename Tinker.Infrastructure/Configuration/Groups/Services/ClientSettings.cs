using FluentValidation;
using Tinker.Infrastructure.Configuration.Base;

namespace Tinker.Infrastructure.Configuration.Groups.Services;

public class ClientSettings : ISettings
{
    public string ApiBaseUrl { get; set; } = string.Empty;
    public int TokenExpirationMinutes { get; set; } = 120;
    public bool RequireTwoFactor { get; set; }

    public void Validate()
    {
        var validator = new ClientSettingsValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new FluentValidation.ValidationException("Client settings validation failed", result.Errors);
    }
}

public class ClientSettingsValidator : AbstractValidator<ClientSettings>
{
    public ClientSettingsValidator()
    {
        RuleFor(x => x.ApiBaseUrl).NotEmpty().Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _));
        RuleFor(x => x.TokenExpirationMinutes).InclusiveBetween(5, 1440);
    }
}