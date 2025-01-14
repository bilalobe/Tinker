using FluentValidation;

namespace Tinker.Core.Application.Compliance.Commands.CreateComplanceLog;

public class CreateComplianceLogCommandValidator : AbstractValidator<CreateComplianceLogCommand>
{
    public CreateComplianceLogCommandValidator()
    {
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
    }
}