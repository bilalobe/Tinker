using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Suppliers.Entities;
using Tinker.Core.Domain.Suppliers.Repositories;
using Tinker.Core.Domain.Suppliers.ValueObjects;
using Tinker.Core.Services.Suppliers.Interfaces;
using Tinker.Shared.Exceptions;

namespace Tinker.Core.Services.Suppliers;

public class SupplierService(ISupplierRepository supplierRepository, ILogger<SupplierService> logger)
    : ISupplierService
{
    public async Task CreateSupplier(Supplier supplier)
    {
        await supplierRepository.AddAsync(supplier);
        logger.LogInformation("Supplier {SupplierId} created successfully", supplier.Id);
    }

    public async Task UpdateContactDetails(SupplierId supplierId, string contactDetails)
    {
        var supplier = await supplierRepository.GetByIdAsync(supplierId.Value);
        if (supplier == null)
            throw new NotFoundException($"Supplier {supplierId} not found");

        supplier.UpdateContactDetails(contactDetails);
        await supplierRepository.UpdateAsync(supplier);
        logger.LogInformation("Contact details updated for supplier {SupplierId}", supplierId);
    }
}