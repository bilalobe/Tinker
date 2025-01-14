using Tinker.Shared.DTOs.Orders;

namespace Tinker.Infrastructure.Core.State.Models;

public class OrderStateModel
{
    public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    public OrderDto? CurrentOrder { get; set; }
    public bool IsLoading { get; set; }
    public string? Error { get; set; }
}