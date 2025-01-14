using FluentValidation;

namespace Tinker.Core.Application.Checkout.Commands.ProcessCheckout;

public class ProcessCheckoutCommandValidator : AbstractValidator<ProcessCheckoutCommand>
{
    public ProcessCheckoutCommandValidator()
    {
        RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("Customer ID is required.");
        RuleFor(x => x.Order.Items).NotEmpty().WithMessage("Order must have at least one item.");
        RuleForEach(x => x.Order.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ProductId).NotEmpty().WithMessage("Product ID is required.");
            items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}