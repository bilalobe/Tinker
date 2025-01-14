namespace Tinker.Shared.DTOs.Users;

public record UserDto
{
    public Guid Id { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsMfaEnabled { get; init; }
    public string MembershipTier { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}