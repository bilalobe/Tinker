namespace Tinker.Core.Security.Models;

public record RoleDefinition
{
    public string Name { get; init; }
    public string[] Permissions { get; init; }
    public bool RequiresMfa { get; init; }
    public string[] AllowedOperations { get; init; }
    public Dictionary<string, string> Claims { get; set; }
}