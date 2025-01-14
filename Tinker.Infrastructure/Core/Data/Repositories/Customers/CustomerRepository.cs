using Microsoft.EntityFrameworkCore;
using Tinker.Infrastructure.Core.Data.Context;

namespace Tinker.Infrastructure.Core.Data.Repositories.Customers;

public class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await context.Customers.FindAsync(id);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await context.Customers.ToListAsync();
    }

    public async Task AddAsync(Customer customer)
    {
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        context.Customers.Update(customer);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Customer customer)
    {
        context.Customers.Remove(customer);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null)
    {
        return await context.Customers.AnyAsync(c =>
            c.Email == email && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public async Task<bool> HasActiveOrdersAsync(int customerId)
    {
        return await context.Orders.AnyAsync(o => o.CustomerId == customerId && o.Status != "Completed");
    }

    public async Task<IEnumerable<Customer>> SearchAsync(string searchTerm)
    {
        return await context.Customers
            .Where(c => c.Name.Contains(searchTerm) || c.Email.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdWithOrdersAsync(int id)
    {
        return await context.Customers
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}