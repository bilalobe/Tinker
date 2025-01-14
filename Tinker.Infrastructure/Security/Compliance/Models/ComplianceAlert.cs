namespace Tinker.Infrastructure.Security.Compliance.Models;

public class ComplianceAlert
{
    internal string? Reference;

    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public DateTime Date { get; set; }
}