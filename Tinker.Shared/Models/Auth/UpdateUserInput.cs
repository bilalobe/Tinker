using System.ComponentModel.DataAnnotations;

namespace Tinker.Shared.Models.Auth;

public class UpdateUserInput
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 3)]
    public string? UserName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }
}