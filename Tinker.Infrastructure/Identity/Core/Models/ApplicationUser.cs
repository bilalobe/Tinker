using Microsoft.AspNetCore.Identity;

namespace Tinker.Infrastructure.Identity.Core.Models;

public class ApplicationUser(
    object?      roles,
    string       email,
    string       userName,
    string?      phoneNumber,
    DateTime     created,
    bool         isActive,
    UserProfile? profile) : IdentityUser
{
    public readonly object? Roles = roles;
    internal new string Email = email;
    internal new string? PhoneNumber = phoneNumber;
    internal new string UserName = userName;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime Created { get; set; } = created;
    public bool IsActive { get; set; } = isActive;
    public UserProfile? Profile { get; set; } = profile;
}