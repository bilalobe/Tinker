using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Customers.Entities;
using Tinker.Core.Domain.Customers.ValueObjects;
using Tinker.Core.Services.Customers.Interfaces;

namespace Tinker.Core.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler(
    ICustomerService                      customerService,
    ILogger<CreateCustomerCommandHandler> logger)
    : IRequestHandler<CreateCustomerCommand, Result<>>
{
    public async Task<Result> Handle(CreateCustomerCommand request, CancellationToken ct)
    {
        try
        {
            var customer = new Customer(CustomerId.New(), request.Name, request.Email, request.PhoneNumber);
            await customerService.CreateCustomer(customer);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating customer");
            return Result.Failure(ex.Message);
        }
    }
}