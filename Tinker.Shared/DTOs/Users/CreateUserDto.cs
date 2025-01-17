namespace Tinker.Shared.DTOs.Users;

public record CreateUserDto
{
    public string UserName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
}