namespace Tinker.Shared.DTOs.Suppliers;

public record SupplierDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string ContactDetails { get; init; }
}