using FluentValidation;

namespace Tinker.Core.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[\d\s-()]+$").WithMessage("A valid phone number is required.");
    }
}