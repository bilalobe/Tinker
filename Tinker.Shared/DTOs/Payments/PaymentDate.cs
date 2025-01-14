namespace Tinker.Shared.DTOs.Payments;

public class PaymentDate(DateTime date, string description)
{
    public DateTime Date { get; set; } = date;
    public string Description { get; set; } = description;
}