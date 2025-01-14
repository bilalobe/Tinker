namespace Tinker.Infrastructure.Configuration.Base;

using FluentValidation;
using FluentValidation.Results;
using Tinker.Shared.Exceptions;

public abstract class SettingsValidatorBase<T> : AbstractValidator<T> where T : ISettings
{
    protected SettingsValidatorBase()
    {
        ConfigureBaseValidation();
    }

    protected virtual void ConfigureBaseValidation()
    {
        RuleFor(x => x).NotNull().WithMessage(ValidationMessages.RequiredField);
    }

    protected ValidationResult ValidateSettings(T settings, string settingsName)
    {
        var result = Validate(settings);
        if (result.IsValid) return result;
        IEnumerable<ValidationError> errors = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
        throw new System.ComponentModel.DataAnnotations.ValidationException($"{settingsName} validation failed", errors);
    }

    protected void ValidateTimeSpan(IRuleBuilder<T, TimeSpan> rule, TimeSpan min, TimeSpan max)
    {
        rule.Must(x => x >= min && x <= max)
            .WithMessage(ValidationMessages.InvalidTimespan);
    }

    protected void ValidateUrl(IRuleBuilder<T, string> rule)
    {
        rule.Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(ValidationMessages.InvalidUrl);
    }
}
