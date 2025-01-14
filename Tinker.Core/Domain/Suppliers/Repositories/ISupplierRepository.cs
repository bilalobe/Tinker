using Tinker.Core.Domain.Suppliers.Entities;

namespace Tinker.Core.Domain.Suppliers.Repositories;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(int id);
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task AddAsync(Supplier    supplier);
    Task UpdateAsync(Supplier supplier);
    Task DeleteAsync(int      id);
}