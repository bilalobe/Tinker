namespace Tinker.Shared.DTOs.Users;

public record UpdateUserDto
{
    public Guid Id { get; init; }
    public string? PhoneNumber { get; init; }
}