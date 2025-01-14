using System.ComponentModel.DataAnnotations;

namespace Tinker.Shared.Models.Auth;

public class LoginInput
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}