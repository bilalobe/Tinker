using FluentValidation;

namespace Tinker.Core.Application.Inventory.Commands.UpdateStock;

public class UpdateStockCommandValidator : AbstractValidator<UpdateStockCommand>
{
    public UpdateStockCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Product ID must be greater than 0.");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
        RuleFor(x => x.Operation).NotEmpty().WithMessage("Operation is required.")
            .Must(op => op == "Add" || op == "Remove").WithMessage("Operation must be either 'Add' or 'Remove'.");
    }
}