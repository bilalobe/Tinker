using Microsoft.AspNetCore.Authorization;

namespace Tinker.Client.Infrastructure.Authorization;

public static class Policies
{
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        options.AddPolicy("CanManageInventory", policy =>
            policy.RequireRole("InventoryManager", "Admin"));

        options.AddPolicy("CanViewReports", policy =>
            policy.RequireRole("Admin", "Manager"));

        options.AddPolicy("CanManageUsers", policy =>
            policy.RequireRole("Admin"));

        options.AddPolicy("CanProcessSales", policy =>
            policy.RequireRole("SalesPerson", "Admin", "Manager"));

        options.AddPolicy("CanManageProducts", policy =>
            policy.RequireRole("ProductManager", "Admin", "InventoryManager"));
    }
}