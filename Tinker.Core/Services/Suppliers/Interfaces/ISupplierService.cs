using Tinker.Core.Domain.Suppliers.Entities;
using Tinker.Core.Domain.Suppliers.ValueObjects;

namespace Tinker.Core.Services.Suppliers.Interfaces;

public interface ISupplierService
{
    Task CreateSupplier(Supplier         supplier);
    Task UpdateContactDetails(SupplierId supplierId, string contactDetails);
}