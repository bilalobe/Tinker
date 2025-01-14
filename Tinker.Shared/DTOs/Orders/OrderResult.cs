namespace Tinker.Shared.DTOs.Orders;

public class OrderResult
{
    public bool Success { get; set; }
    public int OrderId { get; set; }
    public string Message { get; set; } = string.Empty;
}