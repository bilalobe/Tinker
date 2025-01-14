using FluentValidation;

namespace Tinker.Core.Domain.Batch.Validators;

public class BatchValidator : AbstractValidator<Batch>
{
    public BatchValidator()
    {
        RuleFor(x => x.BatchNumber).NotEmpty().WithMessage("Batch number is required.");
        RuleFor(x => x.ExpiryDate).GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");
    }
}