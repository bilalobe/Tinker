using FluentValidation;
using Tinker.Core.Domain.Inventory.Aggregates;

namespace Tinker.Core.Domain.Inventory.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.Reference).NotEmpty().WithMessage("Product reference is required.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product price must be greater than 0.");
        RuleFor(x => x.Quantity.Value).GreaterThanOrEqualTo(0)
            .WithMessage("Product quantity must be greater than or equal to 0.");
    }
}