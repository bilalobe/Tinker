using FluentValidation;
using Tinker.Infrastructure.Configuration.Base;

namespace Tinker.Infrastructure.Configuration.Groups.Services;

public class EmailSettings : ISettings
{
    public bool Enabled { get; set; }
    public string SmtpServer { get; set; } = string.Empty;
    public int Port { get; set; }
    public string FromAddress { get; set; } = string.Empty;

    public void Validate()
    {
        var validator = new EmailSettingsValidator();
        var result = validator.Validate(this);
        if (!result.IsValid)
            throw new FluentValidation.ValidationException("Email settings validation failed", result.Errors);
    }
}

public class EmailSettingsValidator : AbstractValidator<EmailSettings>
{
    public EmailSettingsValidator()
    {
        RuleFor(x => x.SmtpServer).NotEmpty().When(x => x.Enabled);
        RuleFor(x => x.Port).InclusiveBetween(1, 65535).When(x => x.Enabled);
        RuleFor(x => x.FromAddress).EmailAddress().When(x => x.Enabled);
    }
}