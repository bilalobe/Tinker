using Tinker.Core.Domain.Customers.Entities;

namespace Tinker.Core.Domain.Customers.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task AddAsync(Customer                         customer);
    Task UpdateAsync(Customer                      customer);
    Task DeleteAsync(Customer                      customer);
    Task<bool> ExistsByEmailAsync(string           email, int? excludeId = null);
    Task<bool> HasActiveOrdersAsync(int            customerId);
    Task<IEnumerable<Customer>> SearchAsync(string searchTerm);
    Task<Customer?> GetByIdWithOrdersAsync(int     id);
}