namespace Tinker.Core.Security.Configuration;

public class AuthSettings
{
    public JwtOptions Jwt { get; init; } = new();
    public IdentityOptions Identity { get; init; } = new();
    public SecurityOptions Security { get; init; } = new();
}