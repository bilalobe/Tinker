using Tinker.Core.Domain.Customers.Entities;
using Tinker.Shared.DTOs.Customers;

namespace Tinker.Core.Services.Customers.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetCustomers();
    Task<CustomerDto?> GetCustomerById(int                id);
    Task AddCustomer(CustomerDto                          customerDto);
    Task UpdateCustomer(CustomerDto                       customerDto);
    Task<Customer?> GetCustomerEntityById(int             id);
    Task UpdateCustomerEntity(Customer                    customer);
    Task DeleteCustomer(int                               id);
    Task<IEnumerable<CustomerDto>> SearchCustomers(string searchTerm);
    Task<CustomerStatistics> GetCustomerStatistics(int    customerId);
}