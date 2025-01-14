using GreenDonut;
using MediatR;
using Microsoft.Extensions.Logging;
using Tinker.Core.Domain.Suppliers.Entities;
using Tinker.Core.Domain.Suppliers.ValueObjects;
using Tinker.Core.Services.Suppliers.Interfaces;

namespace Tinker.Core.Application.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler(
    ISupplierService                      supplierService,
    ILogger<CreateSupplierCommandHandler> logger)
    : IRequestHandler<CreateSupplierCommand, Result<>>
{
    public async Task<Result> Handle(CreateSupplierCommand request, CancellationToken ct)
    {
        try
        {
            var supplier = new Supplier(SupplierId.New(), request.Name, request.ContactDetails);
            await supplierService.CreateSupplier(supplier);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating supplier");
            return Result.Failure(ex.Message);
        }
    }
}