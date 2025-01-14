using FluentValidation;
using Tinker.Core.Domain.Customers.Entities;

namespace Tinker.Core.Domain.Customers.Validators;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Customer name is required.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Customer email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Customer phone number is required.")
            .Matches(@"^\+?[\d\s-()]+$").WithMessage("A valid phone number is required.");
    }
}