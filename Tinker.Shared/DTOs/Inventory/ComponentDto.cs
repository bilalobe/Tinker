namespace Tinker.Shared.DTOs.Inventory;

public record ComponentDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
}