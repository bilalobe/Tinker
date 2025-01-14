using Tinker.Shared.DTOs.Customers;
using Tinker.Shared.Exceptions;

namespace Tinker.Server.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public class CustomerMutations
{
    [Error(typeof(ValidationException))]
    public async Task<CustomerPayload> CreateCustomer(
        [Service] ICustomerService customerService,
        CustomerDto                input)
    {
        await customerService.AddCustomer(input);
        return new CustomerPayload(input);
    }

    [Error(typeof(ValidationException))]
    [Error(typeof(NotFoundException))]
    public async Task<CustomerPayload> UpdateCustomer(
        [Service] ICustomerService customerService,
        CustomerDto                input)
    {
        await customerService.UpdateCustomer(input);
        return new CustomerPayload(input);
    }

    [Error(typeof(ValidationException))]
    [Error(typeof(NotFoundException))]
    public async Task<CustomerPayload> DeleteCustomer(
        [Service] ICustomerService customerService,
        int                        customerId)
    {
        await customerService.DeleteCustomer(customerId);
        return new CustomerPayload(new CustomerDto
        {
            Id = customerId,
            Name = string.Empty,
            Email = string.Empty,
            PhoneNumber = string.Empty,
            MembershipTier = string.Empty
        });
    }
}