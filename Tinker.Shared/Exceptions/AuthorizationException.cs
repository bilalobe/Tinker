namespace Tinker.Shared.Exceptions;

public class AuthorizationException(
    string  message,
    string? requiredPermission = null,
    string? userRole           = null,
    string? resource           = null)
    : BusinessException(message,
        "AUTHORIZATION_ERROR",
        new Dictionary<string, object>
        {
            ["requiredPermission"] = requiredPermission ?? "unknown",
            ["userRole"] = userRole ?? "unknown",
            ["resource"] = resource ?? "unknown"
        })
{
    public string? RequiredPermission { get; } = requiredPermission;
    public string? UserRole { get; } = userRole;
    public string? Resource { get; } = resource;
}