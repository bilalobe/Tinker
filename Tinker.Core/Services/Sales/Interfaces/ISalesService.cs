using Tinker.Shared.DTOs.Orders;

namespace Tinker.Core.Services.Sales.Interfaces;

public interface ISalesService
{
    Task<OrderDto> ProcessSale(OrderDto orderDto);
}