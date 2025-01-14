namespace Tinker.Shared.DTOs.Payments;

public class PaymentDto
{
    public int OrderId { get; set; }
    public string Method { get; set; } = string.Empty;
}