using Tinker.Core.Security.Models;

namespace Tinker.Core.Security.Policies;

public static class RoleConfiguration
{
    public static readonly IReadOnlyDictionary<string, RoleDefinition> Roles = new Dictionary<string, RoleDefinition>
    {
        ["Admin"] = new()
        {
            Name = "Admin",
            Permissions = ["all"],
            RequiresMfa = true,
            AllowedOperations = ["read", "write", "delete"],
            Claims = new Dictionary<string, string>
            {
                ["SecurityLevel"] = "3",
                ["AccessLevel"] = "Full",
                ["SystemAccess"] = "true"
            }
        },
        ["Pharmacist"] = new()
        {
            Name = "Pharmacist",
            Permissions = ["rx.dispense", "rx.verify", "inventory.read"],
            RequiresMfa = true,
            AllowedOperations = ["read", "write"],
            Claims = new Dictionary<string, string>
            {
                ["SecurityLevel"] = "2",
                ["RxLicense"] = "true",
                ["TrainingComplete"] = "true"
            }
        },
        ["InventoryManager"] = new()
        {
            Name = "InventoryManager",
            Permissions = ["inventory.manage", "reports.read"],
            RequiresMfa = false,
            AllowedOperations = ["read", "write"],
            Claims = new Dictionary<string, string>
            {
                ["SecurityLevel"] = "1",
                ["InventoryAccess"] = "true"
            }
        }
    };
}