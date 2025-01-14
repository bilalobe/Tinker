using Tinker.Shared.DTOs.Customers;

namespace Tinker.Server.GraphQL.Queries;

[ExtendObjectType("Query")]
public class CustomerQueries
{
    public async Task<IEnumerable<CustomerDto>> GetCustomers(
        [Service] ICustomerService customerService)
    {
        return await customerService.GetCustomers();
    }

    public async Task<CustomerDto?> GetCustomer(
        [Service] ICustomerService customerService,
        int                        id)
    {
        return await customerService.GetCustomerById(id);
    }

    public async Task<CustomerStatistics> GetCustomerStatistics(
        [Service] ICustomerService customerService,
        int                        customerId)
    {
        return await customerService.GetCustomerStatistics(customerId);
    }
}