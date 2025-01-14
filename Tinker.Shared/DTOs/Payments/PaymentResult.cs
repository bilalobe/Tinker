namespace Tinker.Shared.DTOs.Payments;

public class PaymentResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}