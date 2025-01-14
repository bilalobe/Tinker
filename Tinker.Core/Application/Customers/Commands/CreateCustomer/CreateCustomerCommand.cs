using GreenDonut;
using MediatR;

namespace Tinker.Core.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand : IRequest<Result<>>
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
}