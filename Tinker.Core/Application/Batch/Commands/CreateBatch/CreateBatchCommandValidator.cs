using FluentValidation;

namespace Tinker.Core.Application.Batch.Commands.CreateBatch;

public class CreateBatchCommandValidator : AbstractValidator<CreateBatchCommand>
{
    public CreateBatchCommandValidator()
    {
        RuleFor(x => x.BatchNumber).NotEmpty().WithMessage("Batch number is required.");
        RuleFor(x => x.ExpiryDate).GreaterThan(DateTime.UtcNow).WithMessage("Expiry date must be in the future.");
    }
}