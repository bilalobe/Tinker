using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Services.Invoices.Interfaces;

public interface IInvoiceService
{
    Task<byte[]> GenerateInvoicePdf(OrderDto order);
}